using UnityEngine;
using Core.Pool;
using Time;

namespace Combat.Weapons
{
    public abstract class WeaponStrategySO : ScriptableObject
    {
        public string WeaponName;
        public float Cooldown = 0.2f;

        // Новая сигнатура абстрактного метода
        public abstract GameObject Fire(Vector3 position, Quaternion rotation, PoolManager poolManager, TimeManager timeManager, bool isEnemy); 
    }
}