using Entities.Components;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class EnemyHealthBar : MonoBehaviour
    {
        [SerializeField] private Slider _healthSlider;
        [SerializeField] private Health _enemyHealth; // Ссылка на здоровье конкретно ЭТОГО врага

        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
            
            // Если забыли прокинуть здоровье руками, пытаемся найти его на родителе
            if (_enemyHealth == null) _enemyHealth = GetComponentInParent<Health>();
        }

        private void OnEnable()
        {
            if (_enemyHealth != null)
            {
                _enemyHealth.OnHealthChanged += UpdateBar;
                // Инициализируем начальное значение
                UpdateBar(_enemyHealth.CurrentHealth, _enemyHealth.MaxHealth);
            }
        }

        private void OnDisable()
        {
            if (_enemyHealth != null) _enemyHealth.OnHealthChanged -= UpdateBar;
        }

        private void UpdateBar(float current, float max)
        {
            if (_healthSlider != null)
            {
                _healthSlider.maxValue = max;
                _healthSlider.value = current;
            }
        }

        // Эффект биллборда — разворачиваем полоску к камере в каждом кадре
        private void LateUpdate()
        {
            if (_mainCamera != null)
            {
                transform.LookAt(transform.position + _mainCamera.transform.rotation * Vector3.forward,
                    _mainCamera.transform.rotation * Vector3.up);
            }
        }
    }
}