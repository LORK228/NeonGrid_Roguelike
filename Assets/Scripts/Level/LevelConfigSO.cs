using UnityEngine;

namespace Level
{
    [CreateAssetMenu(fileName = "Level_1", menuName = "NeonGrid/Level/Level Config")]
    public class LevelConfigSO : ScriptableObject
    {
        [Header("Настройки геометрии")]
        public ArenaThemeSO Theme; // Можно менять визуал арены на каждом уровне!
        public int Width = 20;
        public int Length = 20;
        [Range(0f, 1f)] public float ObstacleDensity = 0.1f;
        [Range(0f, 1f)] public float DecorationDensity = 0.05f;

        [Header("Настройки волн")]
        public int TotalWaves = 3;
        public int BaseEnemies = 5;
        
        [Header("Враги на этом уровне")]
        // Теперь шансы и типы врагов настраиваются уникально для каждого сектора
        public EnemySpawnSetup[] LevelEnemySetups; 
    }
}