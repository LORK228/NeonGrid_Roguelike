using UnityEngine;
using Core.Pool;
using Time;
using Combat.Weapons;

namespace Combat.Modifiers
{
    public class GiantBulletDecorator : FireDecorator
    {
        private readonly float _scaleMultiplier;
        private readonly float _damageMultiplier;

        public GiantBulletDecorator(IFireProcessor wrappedProcessor, float scaleMultiplier, float damageMultiplier) : base(wrappedProcessor) 
        {
            _scaleMultiplier = scaleMultiplier;
            _damageMultiplier = damageMultiplier;
        }

        public override GameObject ExecuteFire(Vector3 position, Quaternion rotation, PoolManager poolManager, TimeManager timeManager, WeaponStrategySO weapon, bool isEnemy)
        {
            GameObject bullet = base.ExecuteFire(position, rotation, poolManager, timeManager, weapon, isEnemy);
            
            if (bullet != null && bullet.TryGetComponent(out Projectile proj))
            {
                // Используем новые безопасные методы
                proj.MultiplyScale(_scaleMultiplier);
                proj.MultiplyDamage(_damageMultiplier);
            }
            
            return bullet;
        }
    }
}