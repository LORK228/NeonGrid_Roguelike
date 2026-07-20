using UnityEngine;
using Core.Pool;
using Time;
using Combat.Weapons;

namespace Combat.Modifiers
{
    public class RearShotDecorator : FireDecorator
    {
        public RearShotDecorator(IFireProcessor wrappedProcessor) : base(wrappedProcessor) { }

        public override GameObject ExecuteFire(Vector3 position, Quaternion rotation, PoolManager poolManager, TimeManager timeManager, WeaponStrategySO weapon, bool isEnemy)
        {
            // Стреляем вперед (как обычно)
            GameObject mainBullet = base.ExecuteFire(position, rotation, poolManager, timeManager, weapon, isEnemy);
            
            // Разворачиваем угол на 180 градусов
            Quaternion rearRotation = rotation * Quaternion.Euler(0, 180f, 0);
            
            // Запускаем вторую пулю назад через WrappedProcessor, чтобы не вызвать бесконечную рекурсию!
            WrappedProcessor.ExecuteFire(position, rearRotation, poolManager, timeManager, weapon, isEnemy);
            
            return mainBullet;
        }
    }
}