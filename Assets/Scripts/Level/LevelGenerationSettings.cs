using UnityEngine;

namespace Level
{
    [System.Serializable]
    public class LevelGenerationSettings
    {
        [Header("Размеры арены")]
        public int Width = 20;
        public int Length = 20;

        [Header("Частота спавна (от 0 до 1)")]
        [Range(0f, 1f)] public float ObstacleDensity = 0.1f;   
        [Range(0f, 1f)] public float DecorationDensity = 0.05f; 

        [Header("Линейная прогрессия боя")]
        public int WavesPerLevel = 3;         // Жестко фиксированное количество волн на любом уровне
        public int BaseEnemies = 3;           // Сколько врагов спавнить в самой первой волне
        public int EnemiesAddedPerWave = 1;   // Плюс X врагов с каждой новой волной
        public int EnemiesAddedPerLevel = 2;  // Плюс X врагов при переходе на новый уровень
    }
}