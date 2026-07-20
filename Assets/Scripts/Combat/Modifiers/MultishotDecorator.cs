using UnityEngine;
using Core.Pool;
using Time;
using Combat.Weapons;

namespace Combat.Modifiers
{
    public class MultishotDecorator : FireDecorator
    {
        private readonly float _spreadAngle;

        public MultishotDecorator(IFireProcessor wrappedProcessor, float spreadAngle) : base(wrappedProcessor) 
        {
            _spreadAngle = spreadAngle;
        }

        public override GameObject ExecuteFire(Vector3 position, Quaternion rotation, PoolManager poolManager, TimeManager timeManager, WeaponStrategySO weapon, bool isEnemy)
        {
            GameObject mainBullet = base.ExecuteFire(position, rotation, poolManager, timeManager, weapon, isEnemy);

            Quaternion leftRotation = rotation * Quaternion.Euler(0, -_spreadAngle, 0);
            WrappedProcessor.ExecuteFire(position, leftRotation, poolManager, timeManager, weapon, isEnemy);

            Quaternion rightRotation = rotation * Quaternion.Euler(0, _spreadAngle, 0);
            WrappedProcessor.ExecuteFire(position, rightRotation, poolManager, timeManager, weapon, isEnemy);

            return mainBullet; 
        }
    }
}