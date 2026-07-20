using System.Threading.Tasks;
using StateMachine.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StateMachine.States
{
    public class GameOverState : IState
    {
        public void Enter()
        {
            Debug.Log("<color=red>[STATE] ИГРОК УНИЧТОЖЕН. GAME OVER.</color>");
            
            // В будущем здесь мы вызовем UI-окно с кнопкой "Рестарт", 
            // а пока просто сделаем автоматическую перезагрузку через 3 секунды.
            RestartRoutine();
        }

        private async void RestartRoutine()
        {
            await Task.Delay(3000); // Даем 3 секунды посмотреть на труп
            
            // Жесткий сброс сцены (Zenject сам всё пересоберет заново)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void Exit()
        {
            // Из этого состояния мы выходим только через перезагрузку сцены
        }
    }
}