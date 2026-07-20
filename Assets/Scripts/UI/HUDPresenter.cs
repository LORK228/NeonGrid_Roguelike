using Combat;
using Entities.Components;
using UnityEngine;
using Zenject;

namespace UI
{
    public class HUDPresenter : MonoBehaviour
    {
        [SerializeField] private HUDView _view; 

        private WaveManager _waveManager;
        private IEntityTarget _playerTarget;
        private Health _playerHealth;

        [Inject]
        public void Construct(WaveManager waveManager, IEntityTarget playerTarget)
        {
            _waveManager = waveManager;
            _playerTarget = playerTarget;
            
            if (_playerTarget.TargetObject != null)
            {
                _playerHealth = _playerTarget.TargetObject.GetComponent<Health>();
            }
        }

        // Подписываемся в Start, когда Zenject УЖЕ точно передал все зависимости
        private void Start()
        {
            if (_playerHealth != null) 
            {
                _playerHealth.OnHealthChanged += _view.UpdateHealthBar;
                _view.UpdateHealthBar(_playerHealth.CurrentHealth, _playerHealth.MaxHealth);
            }
            
            if (_waveManager != null) _waveManager.OnWaveNotification += _view.SetNotificationText;
            
            // + Слушаем глобальные события
            UIEventBus.OnNotification += _view.SetNotificationText; 
        }

        private void OnDestroy()
        {
            if (_playerHealth != null) _playerHealth.OnHealthChanged -= _view.UpdateHealthBar;
            if (_waveManager != null) _waveManager.OnWaveNotification -= _view.SetNotificationText;
            
            // + Отписываемся
            UIEventBus.OnNotification -= _view.SetNotificationText;
        }
    }
}