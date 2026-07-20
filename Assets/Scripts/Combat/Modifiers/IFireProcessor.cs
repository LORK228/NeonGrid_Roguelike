using UnityEngine;
using Core.Pool;
using Time; // Твое новое пространство имен для времени
using Combat.Weapons; // Для WeaponStrategySO

namespace Combat.Modifiers
{
    public interface IFireProcessor
    {
        GameObject ExecuteFire(Vector3 position, Quaternion rotation, PoolManager poolManager, TimeManager timeManager, WeaponStrategySO weapon, bool isEnemy);
    }
}