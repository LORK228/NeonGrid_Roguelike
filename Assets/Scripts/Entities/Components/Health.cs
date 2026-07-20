using System;
using UnityEngine;

namespace Entities.Components
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private float _maxHealth = 100f;
        private float _currentHealth;

        public float CurrentHealth => _currentHealth;
        public float MaxHealth => _maxHealth;
        
        public event Action<float, float> OnHealthChanged;
        public event Action OnDeath;
        
        [Header("Инициализация")]
        [Tooltip("Включить ТОЛЬКО для Игрока. Враги инициализируются скриптами.")]
        [SerializeField] private bool _initOnStart = false;

        private void Start()
        {
            // Если галочка стоит — инициализируемся сами.
            // Идеально для Игрока, который всегда один на сцене со старта.
            if (_initOnStart) InitHealth();
        }
        
        
        public void InitHealth()
        {
            _currentHealth = _maxHealth;
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }

        public void TakeDamage(float damage)
        {
            if (_currentHealth <= 0) return;
            
            _currentHealth = Mathf.Max(0, _currentHealth - damage);
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
            
            if (_currentHealth <= 0) OnDeath?.Invoke();
        }

        public void RestoreHealthForTimeRewind(float previousHealth)
        {
            _currentHealth = previousHealth;
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
        }
    }
}