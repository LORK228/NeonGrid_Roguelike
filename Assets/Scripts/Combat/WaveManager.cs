using System;
using System.Collections.Generic;
using Entities.Components;
using Level;
using StateMachine.States;
using Time;
using Time.Interfaces;
using Time.Trackers;
using Zenject;
using UnityEngine;

namespace Combat
{
    public enum WaveState
    {
        Inactive,
        Intermission, 
        Combat        
    }

    public class WaveManager : ITickable, ITimeTrackable
    {
        private readonly WaveSpawner _waveSpawner;
        private readonly LevelGenerationSettings _settings;
        private readonly TimeManager _timeManager;
        private LevelContext _currentContext;

        public event Action OnAllWavesCleared;
        public event Action<string> OnWaveNotification;

        private WaveState _state = WaveState.Inactive;
        private int _currentWave = 1;
        private int _totalWaves;
        private float _intermissionTimer;
        private int _levelIndex;
        private ArenaData _arenaData;

        private List<Health> _currentWaveEnemies = new List<Health>();
        
        // Кэшированный массив для Zero-Allocation снимков времени
        private Health[] _currentEnemyArray = new Health[0]; 

        // --- МАШИНА ВРЕМЕНИ ДЛЯ МЕНЕДЖЕРА ---
        private TimeRewindBuffer<WaveSnapshot> _snapshots;

        private struct WaveSnapshot
        {
            public WaveState State;
            public int CurrentWave;
            public float IntermissionTimer;
            public Health[] EnemyReferences; 
        }

        // Инжектим новый WaveSpawner вместо кучи старых зависимостей
        public WaveManager(WaveSpawner waveSpawner, TimeManager timeManager)
        {
            _waveSpawner = waveSpawner;
            _timeManager = timeManager;
            _snapshots = new TimeRewindBuffer<WaveSnapshot>(250);
        }

        public void StartCombat(LevelContext context)
        {
            _currentContext = context;
            _totalWaves = context.Config.TotalWaves; // Берем волны из ручных настроек
            _currentWave = 1;
            
            _currentWaveEnemies.Clear();
            _currentEnemyArray = new Health[0];
            _snapshots.Clear();

            if (_timeManager != null) _timeManager.Register(this);

            EnterIntermission();
        }

        public void StopCombat()
        {
            _state = WaveState.Inactive;
            if (_timeManager != null) _timeManager.Unregister(this);
        }

        public void Tick()
        {
            if (_state == WaveState.Inactive) return;

            int aliveEnemies = GetAliveEnemiesCount();

            if (_state == WaveState.Intermission)
            {
                if (_timeManager != null && _timeManager.IsRewinding) return;

                _intermissionTimer -= UnityEngine.Time.deltaTime;
                UpdateNotificationUI();

                if (_intermissionTimer <= 0)
                {
                    SpawnCurrentWave();
                }
            }
            else if (_state == WaveState.Combat)
            {
                if (aliveEnemies <= 0)
                {
                    if (_timeManager != null && _timeManager.IsRewinding) return;

                    _currentWave++;
                    
                    if (_currentWave > _totalWaves)
                    {
                        _state = WaveState.Inactive;
                        OnWaveNotification?.Invoke("// СЕКТОР ЗАЧИЩЕН. ЗАБЕРИТЕ ФАЙЛ //");
                        OnAllWavesCleared?.Invoke();
                    }
                    else
                    {
                        EnterIntermission();
                    }
                }
            }
        }

        // ==========================================
        // РЕАЛИЗАЦИЯ ИНТЕРФЕЙСА ВРЕМЕНИ
        // ==========================================

        public void RecordState()
        {
            if (_state == WaveState.Inactive) return;

            // Передаем ссылку на закэшированный массив, чтобы избежать GC.Alloc в каждом кадре
            _snapshots.Record(new WaveSnapshot {
                State = _state,
                CurrentWave = _currentWave,
                IntermissionTimer = _intermissionTimer,
                EnemyReferences = _currentEnemyArray
            });
        }

        public void RewindState()
        {
            if (_snapshots.TryRewind(out WaveSnapshot snap))
            {
                _state = snap.State;
                _currentWave = snap.CurrentWave;
                _intermissionTimer = snap.IntermissionTimer;
                
                // Если массив врагов изменился при перемотке (например, отмотали до старта другой волны)
                if (_currentEnemyArray != snap.EnemyReferences)
                {
                    _currentEnemyArray = snap.EnemyReferences;
                    _currentWaveEnemies.Clear();
                    if (_currentEnemyArray != null)
                    {
                        _currentWaveEnemies.AddRange(_currentEnemyArray);
                    }
                }

                UpdateNotificationUI();
            }
        }

        public void ClearHistory() => _snapshots.Clear();
        public void StartRewind() { }
        public void StopRewind() { }

        // ==========================================

        private void EnterIntermission()
        {
            _state = WaveState.Intermission;
            _intermissionTimer = 3f;
            UpdateNotificationUI(); 
        }

        private void EnterCombatState()
        {
            _state = WaveState.Combat;
            UpdateNotificationUI(); 
        }

        private int GetAliveEnemiesCount()
        {
            int count = 0;
            for (int i = 0; i < _currentWaveEnemies.Count; i++)
            {
                var enemy = _currentWaveEnemies[i];
                if (enemy != null && enemy.gameObject.activeInHierarchy && enemy.CurrentHealth > 0)
                {
                    count++;
                }
            }
            return count;
        }

        private void SpawnCurrentWave()
        {
            _currentWaveEnemies.Clear();
            
            // Передаем контекст в спавнер
            var spawned = _waveSpawner.SpawnWave(_currentContext, _currentWave);
            _currentWaveEnemies.AddRange(spawned);

            _currentEnemyArray = _currentWaveEnemies.ToArray();

            EnterCombatState();
        }
        
        private void UpdateNotificationUI()
        {
            if (_state == WaveState.Combat)
            {
                OnWaveNotification?.Invoke($"--- СИСТЕМА АТАКОВАНА: ВОЛНА {_currentWave}/{_totalWaves} ---");
            }
            else if (_state == WaveState.Intermission)
            {
                OnWaveNotification?.Invoke($"Сектор {_levelIndex} // Волна {_currentWave} через {_intermissionTimer:F1} сек...");
            }
        }
    }
}