using Core.Pool;
using Entities.Components;
using UnityEngine;

// Для пулов

namespace Entities.Enemies
{
    public class HoundEnemy : EnemyBase
    {
        [Header("Настройки Гончей")]
        public float Damage = 15f;
        public float AttackRange = 1.5f; // Дистанция подрыва

        protected override void Awake()
        {
            base.Awake();
            // Заставляем агента останавливаться чуть раньше, чем он врежется в нас
            if (_agent != null) _agent.stoppingDistance = AttackRange - 0.2f;
        }

        protected override void ExecuteBehavior()
        {
            float distance = Vector3.Distance(transform.position, _target.Position);

            if (distance <= AttackRange)    
            {
                BlowUp();
            }
            else
            {
                _agent.isStopped = false;
                // ИСПОЛЬЗУЕМ НОВЫЙ МЕТОД:
                MoveTo(_target.Position);
            }
        }

        private void BlowUp()
        {
            // Пытаемся взять компонент Health у нашего интерфейса цели
            if (_target.TargetObject != null && _target.TargetObject.TryGetComponent(out Health targetHealth))
            {
                targetHealth.TakeDamage(Damage);
                Debug.Log($"<color=red>Гончая взорвалась! Здоровье игрока: {targetHealth.CurrentHealth}</color>");
            }
            
            // Уничтожаемся (возвращаемся в пул)
            if (TryGetComponent(out PooledObject po)) po.Return();
            else gameObject.SetActive(false);
        }
    }
}