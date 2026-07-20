using UnityEngine;
using Zenject;
using Core.Pool;
using Combat.Weapons;
using Combat.Modifiers;

namespace Entities.Enemies
{
    public class FirewallEnemy : EnemyBase
    {
        [Header("Настройки Брандмауэра")]
        public float StoppingDistance = 5f;  
        public float ShootingRange = 15f;    
        public float FireRate = 2f;
        
        [Header("Видимость (Raycast)")]
        public LayerMask SightMask; 

        [Header("Оружие (Стратегия)")]
        public WeaponStrategySO WeaponStrategy; // Удалили ProjectilePrefab и Speed!
        public Transform FirePoint;

        private PoolManager _poolManager;
        private BaseFireProcessor _fireProcessor;
        private float _nextFireTime;

        // Инжектим зависимости специально для стрельбы
        [Inject]
        public void ConstructShooter(PoolManager poolManager, BaseFireProcessor fireProcessor)
        {
            _poolManager = poolManager;
            _fireProcessor = fireProcessor;
        }

        protected override void ExecuteBehavior()
        {
            if (!_agent.enabled) return;

            float distance = Vector3.Distance(transform.position, _target.Position);
            bool hasLineOfSight = HasLineOfSight();

            if (distance > StoppingDistance || !hasLineOfSight)
            {
                _agent.isStopped = false;
                MoveTo(_target.Position);
            }
            else
            {
                _agent.isStopped = true;
            }

            if (hasLineOfSight && distance <= ShootingRange)
            {
                _agent.updateRotation = false; 
                Vector3 lookPoint = new Vector3(_target.Position.x, transform.position.y, _target.Position.z);
                transform.LookAt(lookPoint);
                TryShoot();
            }
            else
            {
                _agent.updateRotation = true; 
            }
        }

        private bool HasLineOfSight()
        {
            Vector3 startPos = FirePoint != null ? FirePoint.position : transform.position; 
            Vector3 targetPos = _target.Position;
            Vector3 direction = targetPos - startPos;

            if (Physics.Raycast(startPos, direction.normalized, out RaycastHit hit, ShootingRange, SightMask))
            {
                if (hit.collider.CompareTag("Player"))
                {
#if UNITY_EDITOR
                    Debug.DrawRay(startPos, direction.normalized * hit.distance, Color.green);
#endif
                    return true; 
                }
                else
                {
#if UNITY_EDITOR
                    Debug.DrawRay(startPos, direction.normalized * hit.distance, Color.red);
#endif
                }
            }
            return false;
        }

        private void TryShoot()
        {
            if (UnityEngine.Time.time >= _nextFireTime && WeaponStrategy != null && _fireProcessor != null)
            {
                _nextFireTime = UnityEngine.Time.time + FireRate;
                
                // Враг стреляет через процессор! Флаг isEnemy = true
                _fireProcessor.ExecuteFire(FirePoint.position, FirePoint.rotation, _poolManager, _timeManager, WeaponStrategy, true);
            }
        }
    }
}