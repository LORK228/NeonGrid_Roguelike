using UnityEngine;
using Level;
using Unity.AI.Navigation; 
using System.Threading.Tasks;
using StateMachine.Interfaces; 
using UI;
using System.Collections.Generic;

namespace StateMachine.States
{
    public class GenerateLevelState : IPayloadedState<int>
    {
        private readonly GameStateMachine _stateMachine;
        private readonly ArenaBuilder _builder;
        private readonly List<LevelConfigSO> _campaignLevels;
        
        private GameObject _previousArenaRoot;

        // Инжектим список уровней вместо одной глобальной настройки
        public GenerateLevelState(
            GameStateMachine stateMachine, 
            ArenaBuilder builder, 
            List<LevelConfigSO> campaignLevels)
        {
            _stateMachine = stateMachine;
            _builder = builder;
            _campaignLevels = campaignLevels;
        }

        public async void Enter(int levelIndex)
        {
            UIEventBus.ShowText($"СИНТЕЗ СЕКТОРА {levelIndex}...");
            await Task.Yield(); 

            if (_previousArenaRoot != null)
            {
                Object.Destroy(_previousArenaRoot);
            }

            // Безопасно получаем конфиг (если уровней 5, а мы на 6-м, загрузим 5-й)
            int configIndex = Mathf.Clamp(levelIndex - 1, 0, _campaignLevels.Count - 1);
            LevelConfigSO currentConfig = _campaignLevels[configIndex];

            int totalCells = currentConfig.Width * currentConfig.Length;
            int obstaclesCount = Mathf.RoundToInt(totalCells * currentConfig.ObstacleDensity);
            int decorationsCount = Mathf.RoundToInt(totalCells * currentConfig.DecorationDensity);

            ArenaData currentArena = _builder
                .Begin($"Sector_{levelIndex}", currentConfig.Theme) // Берем визуальную тему из конфига
                .BuildFloor(currentConfig.Width, currentConfig.Length)
                .BuildWalls(currentConfig.Width, currentConfig.Length)
                .BuildObstacles(obstaclesCount, currentConfig.Width, currentConfig.Length)
                .BuildDecorations(decorationsCount, currentConfig.Width, currentConfig.Length)
                .BuildEnemySpawns(10, currentConfig.Width, currentConfig.Length) 
                .Build();

            _previousArenaRoot = currentArena.ArenaRoot.gameObject;

            NavMeshSurface surface = currentArena.ArenaRoot.gameObject.AddComponent<NavMeshSurface>();
            surface.BuildNavMesh();

            // Передаем конфиг дальше по цепочке
            _stateMachine.Enter<WaveCombatState, LevelContext>(new LevelContext { 
                LevelIndex = levelIndex, 
                ArenaData = currentArena,
                Config = currentConfig 
            });
        }

        public void Exit() { }
    }
}