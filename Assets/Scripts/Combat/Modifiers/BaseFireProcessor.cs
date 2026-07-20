using UnityEngine;
using Core.Pool;
using Time;
using Combat.Weapons;

namespace Combat.Modifiers
{
    public class BaseFireProcessor : IFireProcessor
    {
        public GameObject ExecuteFire(Vector3 position, Quaternion rotation, PoolManager poolManager, TimeManager timeManager, WeaponStrategySO weapon, bool isEnemy)
        {
            // Передаем правильные аргументы в стратегию оружия
            return weapon.Fire(position, rotation, poolManager, timeManager, isEnemy);
        }
    }
}