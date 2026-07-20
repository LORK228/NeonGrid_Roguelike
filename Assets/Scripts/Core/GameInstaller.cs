using System.Collections.Generic;
using Combat;
using Combat.Modifiers;
using Core.Pool;
using Entities.Components;
using Entities.Player;
using Input;
using Level;
using StateMachine;
using Time;
using UnityEngine;
using Zenject;

// Не забываем подключить коллекции

namespace Core
{
    public class GameInstaller : MonoInstaller
    {
        [Header("Глобальные системы")]
        [SerializeField] private TimeManager _timeManager;
    
        [Header("Настройки пулов (Заменит старый Inspector)")]
        [SerializeField] private List<PoolSetup> _poolSetups;
    
        [Header("Компоненты Игрока")]
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private PlayerWeapon _playerWeapon;
        [SerializeField] private ArtifactHolder _artifactHolder;
    
        [Header("Настройки Уровня")]
        [SerializeField] private ArenaThemeSO _startingArenaTheme;
    
        [Header("Кампания (Порядок уровней)")]
        [SerializeField] private List<Level.LevelConfigSO> _campaignLevels;
        
        
        [Header("Настройки стейт-машины")]
        [SerializeField] private StateMachine.GameStateSettings _gameStateSettings;

        public override void InstallBindings()
        {
            // 1. Ввод
#if UNITY_ANDROID || UNITY_IOS
        Container.Bind<IInputProvider>().To<MobileInputProvider>().AsSingle().WithArguments((Joystick)null, (Joystick)null);
#else
            Container.Bind<IInputProvider>().To<DesktopInputProvider>().AsSingle();
#endif

            // 2. Биндим конкретные инстансы Игрока
            Container.BindInterfacesAndSelfTo<PlayerMovement>().FromInstance(_playerMovement).AsSingle();
            Container.BindInstance(_playerWeapon).AsSingle();
            Container.BindInstance(_artifactHolder).AsSingle();
            Container.BindInstance(_timeManager).AsSingle();

            // 3. ЯДРО (Core): Биндим чистые классы C#
        
            // Передаем список настроек пула в контейнер
            Container.BindInstance(_poolSetups).AsSingle();
        
            // BindInterfacesAndSelfTo говорит: "Свяжи этот класс и все его интерфейсы (IInitializable)".
            // NonLazy() говорит: "Не жди, пока его кто-то попросит. Создай его СРАЗУ при старте сцены".
            Container.BindInterfacesAndSelfTo<PoolManager>().AsSingle().NonLazy();
            Container.Bind<BaseFireProcessor>().AsSingle();
        
            // === СЛОЙ 2: ГЕНЕРАЦИЯ АРЕНЫ ===
        
            Container.BindInstance(_startingArenaTheme).AsSingle();
        
            // Биндим список всех уровней в контейнер
            Container.BindInstance(_campaignLevels).AsSingle();

            Container.Bind<ArenaBuilder>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameStateMachine>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameBootstrapper>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<WaveManager>().AsSingle();
            Container.Bind<WaveSpawner>().AsSingle();
            
            // Добавляем настройки в контейнер, чтобы RewardState мог их получить
            Container.BindInstance(_gameStateSettings).AsSingle();
        }
    }
}