using StateMachine;
using StateMachine.States;
using Zenject;

namespace Core
{
    public class GameBootstrapper : IInitializable
    {
        private readonly GameStateMachine _stateMachine;

        public GameBootstrapper(GameStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Initialize()
        {
            // Запускаем великий цикл с Уровня 1
            _stateMachine.Enter<GenerateLevelState, int>(1);
        }
    }
}