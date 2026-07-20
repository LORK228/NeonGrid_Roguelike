using UnityEngine;
using Core.Pool;
using Time;
using Combat.Weapons;

namespace Combat.Modifiers
{
    public class HeavyDamageDecorator : FireDecorator
    {
        private readonly float _damageMultiplier;

        // Добавили аргумент в конструктор
        public HeavyDamageDecorator(IFireProcessor wrappedProcessor, float damageMultiplier) : base(wrappedProcessor) 
        {
            _damageMultiplier = damageMultiplier;
        }

        public override GameObject ExecuteFire(Vector3 position, Quaternion rotation, PoolManager poolManager, TimeManager timeManager, WeaponStrategySO weapon, bool isEnemy)
        {
            GameObject bullet = base.ExecuteFire(position, rotation, poolManager, timeManager, weapon, isEnemy);

            if (bullet != null && bullet.TryGetComponent(out Projectile proj))
            {
                proj.MultiplyDamage(_damageMultiplier); // Используем поле вместо хардкода
            }

            return bullet;
        }
    }
}