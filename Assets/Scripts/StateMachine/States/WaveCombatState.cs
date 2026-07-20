using Combat;
using StateMachine.Interfaces;
using UnityEngine;

namespace StateMachine.States
{
    public class WaveCombatState : IPayloadedState<LevelContext>
    {
        private readonly GameStateMachine _stateMachine;
        private readonly WaveManager _waveManager;

        public WaveCombatState(GameStateMachine stateMachine, WaveManager waveManager)
        {
            _stateMachine = stateMachine;
            _waveManager = waveManager;
        }

        public void Enter(LevelContext context)
        {
            Debug.Log($"[STATE] Бой начался! Уровень: {context.LevelIndex}");

            _waveManager.OnAllWavesCleared += FinishCombat;
            
            // Передаем весь контекст, включая конфиг
            _waveManager.StartCombat(context); 
        }

        private void FinishCombat()
        {
            _waveManager.OnAllWavesCleared -= FinishCombat; 
            _stateMachine.Enter<RewardState>();
        }

        public void Exit()
        {
            _waveManager.StopCombat();
        }
    }
}