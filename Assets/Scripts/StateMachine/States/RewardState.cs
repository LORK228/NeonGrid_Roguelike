using Level;
using StateMachine.Interfaces;
using UnityEngine;
using Zenject;

namespace StateMachine.States
{
    public class RewardState : IState
    {
        private readonly GameStateMachine _stateMachine;
        private readonly DiContainer _container;
        private readonly GameStateSettings _settings;
        
        public static int CurrentLevel = 1;

        // Инжектим зависимости
        public RewardState(GameStateMachine stateMachine, DiContainer container, GameStateSettings settings)
        {
            _stateMachine = stateMachine;
            _container = container;
            _settings = settings;
        }

        public void Enter()
        {
            Debug.Log("[STATE] Спавн сундука. Ждем действий игрока.");
            
            // Подписываемся на открытие сундука
            RewardChest.OnChestOpened += SpawnPortal;
            LevelPortal.OnPortalEntered += GoToNextLevel;

            // Спавним нормальный префаб сундука из настроек
            if (_settings.RewardChestPrefab != null)
            {
                _container.InstantiatePrefab(_settings.RewardChestPrefab, new Vector3(0, 0.5f, 0), Quaternion.identity, null);
            }
            else
            {
                Debug.LogError("[RewardState] Префаб сундука не назначен в GameInstaller!");
            }
        }

        private void SpawnPortal(Vector3 chestPosition)
        {
            // Отписываемся, чтобы не заспавнить портал дважды
            RewardChest.OnChestOpened -= SpawnPortal;
            
            Debug.Log("[STATE] Сундук открыт. Появление портала.");

            // Спавним портал правее от сундука
            Vector3 portalPos = chestPosition + new Vector3(1.5f, 0, 0);
            
            if (_settings.LevelPortalPrefab != null)
            {
                _container.InstantiatePrefab(_settings.LevelPortalPrefab, portalPos, Quaternion.identity, null);
            }
        }

        private void GoToNextLevel()
        {
            LevelPortal.OnPortalEntered -= GoToNextLevel;
            
            CurrentLevel++;
            Debug.Log($"<color=magenta>ПЕРЕХОД НА УРОВЕНЬ {CurrentLevel}</color>");
            
            _stateMachine.Enter<GenerateLevelState, int>(CurrentLevel);
        }

        public void Exit()
        {
            // Страховочная отписка на случай непредвиденного выхода из стейта
            RewardChest.OnChestOpened -= SpawnPortal;
            LevelPortal.OnPortalEntered -= GoToNextLevel;
            
            Debug.Log("[STATE] Игрок забрал лут, генерация следующего уровня.");
        }
    }
}