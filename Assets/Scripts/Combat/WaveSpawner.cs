using System.Collections.Generic;
using Core.Pool;
using Entities.Components;
using Level;
using StateMachine.States;
using UnityEngine;
using UnityEngine.AI;

namespace Combat
{
    public class WaveSpawner
    {
        private readonly PoolManager _poolManager;
        private readonly IEntityTarget _player;
        
        private readonly float _safeSpawnDistance = 7f;

        // Настройки нам больше не нужны при инициализации, мы получаем их динамически
        public WaveSpawner(PoolManager poolManager, IEntityTarget player)
        {
            _poolManager = poolManager;
            _player = player;
        }

        public List<Health> SpawnWave(LevelContext context, int currentWave)
        {
            List<Health> spawnedEnemies = new List<Health>();
            
            // Теперь берем шансы и настройки конкретно для этого уровня
            var config = context.Config;

            // Простая формула для ручного дизайна: Базовые враги + (Номер волны * Добавочные)
            // (Можно добавить поле EnemiesAddedPerWave в LevelConfigSO для большей гибкости)
            int count = config.BaseEnemies + ((currentWave - 1) * 2);

            if (config.LevelEnemySetups == null || config.LevelEnemySetups.Length == 0 || context.ArenaData.EnemySpawnPoints.Count == 0)
                return spawnedEnemies;

            for (int i = 0; i < count; i++)
            {
                Vector3 spawnPos = GetCalculatedSpawnPoint(context.ArenaData.EnemySpawnPoints);
                
                // Передаем уникальные шансы этого уровня
                GameObject prefab = GetRandomEnemyPrefab(config.LevelEnemySetups); 
                
                GameObject enemyObj = _poolManager.SpawnFromPool(prefab, spawnPos, Quaternion.identity);

                CorrectEnemyYPosition(enemyObj, spawnPos);

                if (enemyObj.TryGetComponent(out Health enemyHealth))
                {
                    enemyHealth.InitHealth(); 
                    spawnedEnemies.Add(enemyHealth);
                }
            }

            return spawnedEnemies;
        }

        private Vector3 GetCalculatedSpawnPoint(List<Vector3> allPoints)
        {
            Vector3 rawSpawnPos = GetSafeSpawnPoint(allPoints);
            if (NavMesh.SamplePosition(rawSpawnPos, out NavMeshHit hit, 2.0f, NavMesh.AllAreas))
            {
                return hit.position;
            }
            return rawSpawnPos;
        }

        private void CorrectEnemyYPosition(GameObject enemyObj, Vector3 perfectSpawnPos)
        {
            Collider col = enemyObj.GetComponentInChildren<Collider>();
            if (col != null)
            {
                float yOffset = perfectSpawnPos.y - col.bounds.min.y;
                if (Mathf.Abs(yOffset) > 0.01f)
                {
                    enemyObj.transform.position += Vector3.up * yOffset;
                    if (enemyObj.TryGetComponent(out NavMeshAgent agent) && agent.isActiveAndEnabled)
                    {
                        agent.Warp(enemyObj.transform.position);
                    }
                }
            }
        }

        private Vector3 GetSafeSpawnPoint(List<Vector3> allPoints)
        {
            if (_player == null || !_player.IsActive) return allPoints[Random.Range(0, allPoints.Count)];

            Vector3 playerPos = _player.Position;
            List<Vector3> validPoints = new List<Vector3>();

            foreach (var point in allPoints)
                if (Vector3.Distance(point, playerPos) >= _safeSpawnDistance) validPoints.Add(point);

            if (validPoints.Count > 0) return validPoints[Random.Range(0, validPoints.Count)];

            Vector3 furthestPoint = allPoints[0];
            float maxDistance = 0f;

            foreach (var point in allPoints)
            {
                float dist = Vector3.Distance(point, playerPos);
                if (dist > maxDistance)
                {
                    maxDistance = dist;
                    furthestPoint = point;
                }
            }
            return furthestPoint;
        }

        private GameObject GetRandomEnemyPrefab(Level.EnemySpawnSetup[] setups)
        {
            int totalWeight = 0;
            foreach (var setup in setups) totalWeight += setup.SpawnWeight;

            int randomValue = Random.Range(0, totalWeight);
            int currentWeight = 0;

            foreach (var setup in setups)
            {
                currentWeight += setup.SpawnWeight;
                if (randomValue < currentWeight) return setup.EnemyPrefab;
            }
            return setups[0].EnemyPrefab;
        }
    }
}