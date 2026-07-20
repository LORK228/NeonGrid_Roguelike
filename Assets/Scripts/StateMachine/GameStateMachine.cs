using System;
using System.Collections.Generic;
using Entities.Player;
using StateMachine.Interfaces;
using StateMachine.States;
using Zenject;

// Для доступа к PlayerDeathHandler

namespace StateMachine
{
    // Наследуем интерфейс IDisposable
    public class GameStateMachine : IDisposable
    {
        private Dictionary<Type, IExitableState> _states;
        private IExitableState _activeState;

        public GameStateMachine(DiContainer container)
        {
            _states = new Dictionary<Type, IExitableState>
            {
                [typeof(GenerateLevelState)] = container.Instantiate<GenerateLevelState>(new object[] { this }),
                [typeof(WaveCombatState)] = container.Instantiate<WaveCombatState>(new object[] { this }),
                [typeof(RewardState)] = container.Instantiate<RewardState>(new object[] { this }),
                [typeof(GameOverState)] = container.Instantiate<GameOverState>() 
            };

            // Подписываемся на смерть игрока
            PlayerDeathHandler.OnPlayerDied += HandlePlayerDeath;
        }

        public void Enter<TState>() where TState : class, IState
        {
            TState state = ChangeState<TState>();
            state.Enter();
        }

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadedState<TPayload>
        {
            TState state = ChangeState<TState>();
            state.Enter(payload);
        }

        private TState ChangeState<TState>() where TState : class, IExitableState
        {
            _activeState?.Exit();
            TState state = _states[typeof(TState)] as TState;
            _activeState = state;
            
            return state;
        }

        private void HandlePlayerDeath()
        {
            Enter<GameOverState>();
        }

        // --- ТОТ САМЫЙ МЕТОД ОЧИСТКИ ---
        public void Dispose()
        {
            // Отписываемся от глобального события при уничтожении машины состояний
            PlayerDeathHandler.OnPlayerDied -= HandlePlayerDeath;
        }
    }
}