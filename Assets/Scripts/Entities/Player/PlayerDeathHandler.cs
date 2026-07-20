using System;
using Entities.Components;
using UnityEngine;

namespace Entities.Player
{
    [RequireComponent(typeof(Health))] 
    public class PlayerDeathHandler : MonoBehaviour
    {
        // Глобальное статическое событие
        public static event Action OnPlayerDied; 

        private Health _health;

        private void Awake()
        {
            _health = GetComponent<Health>();
        }

        private void OnEnable()
        {
            _health.OnDeath += Die;
        }

        private void OnDisable()
        {
            _health.OnDeath -= Die;
        }

        private void Die()
        {
            gameObject.SetActive(false);
            
            // Просто рассылаем сигнал всем, кому это интересно
            OnPlayerDied?.Invoke(); 
        }
    }
}