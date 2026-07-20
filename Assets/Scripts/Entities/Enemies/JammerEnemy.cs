using Entities.Enemies;
using Entities.Components;
using UnityEngine;

namespace Entities.Enemies
{
    public class JammerEnemy : EnemyBase
    {
        [Header("Настройки Глушилки")]
        public float AuraRadius = 4f;
        public float DamagePerSecond = 15f;

        protected override void Awake()
        {
            base.Awake();
            if (_agent != null)
            {
                // Глушилка медленная, но неотвратимая
                _agent.speed = 2.5f; 
                _agent.stoppingDistance = 0f; // Всегда пытается подойти вплотную
            }
        }

        protected override void ExecuteBehavior()
        {
            if (!_agent.enabled) return;

            // 1. Всегда ползем к игроку
            MoveTo(_target.Position);

            // 2. Жжем игрока аурой, если он в радиусе
            float distance = Vector3.Distance(transform.position, _target.Position);
            if (distance <= AuraRadius && _target.TargetObject != null)
            {
                if (_target.TargetObject.TryGetComponent(out Health targetHealth))
                {
                    // Наносим урон с учетом времени кадра (DoT - Damage over Time)
                    targetHealth.TakeDamage(DamagePerSecond * UnityEngine.Time.deltaTime);
                }
            }
        }

        // Для удобства настройки в редакторе нарисуем радиус ауры
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f); // Оранжевый полупрозрачный
            Gizmos.DrawSphere(transform.position, AuraRadius);
        }
#endif
    }
}