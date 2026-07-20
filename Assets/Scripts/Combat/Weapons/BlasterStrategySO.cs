using UnityEngine;
using Core.Pool;
using Time;

namespace Combat.Weapons
{
    [CreateAssetMenu(fileName = "New Blaster", menuName = "Weapons/Blaster")]
    public class BlasterStrategySO : WeaponStrategySO
    {
        [Header("Настройки Бластера")]
        public GameObject ProjectilePrefab;
        public float ProjectileSpeed = 20f;
        
        // Убрали хардкод урона!
        [SerializeField] private float _damage = 25f; 

        public override GameObject Fire(Vector3 position, Quaternion rotation, PoolManager poolManager, TimeManager timeManager, bool isEnemy)
        {
            GameObject bullet = poolManager.SpawnFromPool(ProjectilePrefab, position, rotation);
            
            if (bullet.TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = false;
                rb.linearVelocity = rotation * Vector3.forward * ProjectileSpeed;
            }
    
            if (bullet.TryGetComponent(out Projectile proj))
            {
                proj.Init(timeManager, _damage, isEnemy); // Используем поле из инспектора
            }

            if (bullet.TryGetComponent(out TimeBody tb)) tb.ClearHistory();
    
            return bullet;
        }
    }
}