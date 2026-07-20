using UnityEngine;

namespace Level
{
    // Структуру оставляем, так как она теперь используется в LevelConfigSO
    [System.Serializable]
    public class EnemySpawnSetup
    {
        public GameObject EnemyPrefab;
        [Range(1, 100)] public int SpawnWeight = 50; 
    }

    [CreateAssetMenu(fileName = "New Arena Theme", menuName = "NeonGrid/Level/ArenaTheme")]
    public class ArenaThemeSO : ScriptableObject
    {
        [Header("Основа (Геометрия)")]
        public GameObject FloorTilePrefab;
        public GameObject WallPrefab;
        
        [Header("Геймплей (Укрытия)")]
        public GameObject[] ObstaclePrefabs;
        
        [Header("Атмосфера (Визуал)")]
        public GameObject[] DecorationPrefabs;
        
        // Блок "Враги (Настройка шансов)" УДАЛЕН.
        // Теперь тема отвечает ТОЛЬКО за внешний вид уровня.
    }
}