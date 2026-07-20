using UnityEngine;
using Core.Pool;
using Time;
using Combat.Weapons;

namespace Combat.Modifiers
{
    public abstract class FireDecorator : IFireProcessor
    {
        protected readonly IFireProcessor WrappedProcessor;

        protected FireDecorator(IFireProcessor wrappedProcessor)
        {
            WrappedProcessor = wrappedProcessor;
        }

        public virtual GameObject ExecuteFire(Vector3 position, Quaternion rotation, PoolManager poolManager, TimeManager timeManager, WeaponStrategySO weapon, bool isEnemy)
        {
            // Делегируем вызов дальше по цепочке с новыми аргументами
            return WrappedProcessor.ExecuteFire(position, rotation, poolManager, timeManager, weapon, isEnemy);
        }
    }
}