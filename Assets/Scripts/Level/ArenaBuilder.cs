using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using Zenject;

namespace Level
{
    public class ArenaBuilder
    {
        private readonly DiContainer _container;
        
        private ArenaThemeSO _currentTheme;
        private ArenaData _arenaData; // Теперь Строитель собирает этот объект
        private HashSet<Vector2> _usedPositions = new HashSet<Vector2>();

        public ArenaBuilder(DiContainer container)
        {
            _container = container;
        }

        public ArenaBuilder Begin(string arenaName, ArenaThemeSO theme)
        {
            _currentTheme = theme;
            
            _arenaData = new ArenaData();
            _arenaData.ArenaRoot = new GameObject($"[Arena] {arenaName}").transform;
            
            _usedPositions.Clear();
            
            // Сразу бронируем центр (0,0) под Игрока, чтобы там ничего не заспавнилось
            _usedPositions.Add(Vector2.zero); 
            _arenaData.PlayerSpawnPoint = new Vector3(0, 0.5f, 0);

            return this;
        }

        // 1. УМНЫЙ ПОЛ: Создаем 1 объект и растягиваем его (0 лагов)
        public ArenaBuilder BuildFloor(int width, int length)
        {
            // Ожидается, что FloorTilePrefab - это стандартный куб Unity (1x1)
            GameObject floor = Spawn(_currentTheme.FloorTilePrefab, Vector3.zero);
            
            // Растягиваем его под нужные размеры арены. Высота пола = 1.
            floor.transform.localScale = new Vector3(width, 1f, length);
            
            // Сдвигаем вниз на половину высоты, чтобы нулевая координата Y (0) была ровно поверхностью пола
            floor.transform.position = new Vector3(0, -0.5f, 0); 
            
            return this;
        }

        // 2. СТЕНЫ: Генерируем ровно по периметру
        public ArenaBuilder BuildWalls(int width, int length)
        {
            float halfW = width / 2f;
            float halfL = length / 2f;

            // Левая и Правая стены (идем по оси Z)
            for (float z = -halfL; z <= halfL; z++)
            {
                Spawn(_currentTheme.WallPrefab, new Vector3(-halfW, 0.5f, z)); // Левая
                Spawn(_currentTheme.WallPrefab, new Vector3(halfW, 0.5f, z));  // Правая
            }

            // Верхняя и Нижняя стены (идем по оси X)
            // Начинаем с +1 и заканчиваем на -1, чтобы не дублировать кубики в углах
            for (float x = -halfW + 1; x < halfW; x++) 
            {
                Spawn(_currentTheme.WallPrefab, new Vector3(x, 0.5f, halfL));  // Верхняя
                Spawn(_currentTheme.WallPrefab, new Vector3(x, 0.5f, -halfL)); // Нижняя
            }

            return this;
        }

        // 3. ПРЕПЯТСТВИЯ: Жесткая привязка к сетке, чтобы не пересекались
        public ArenaBuilder BuildObstacles(int count, int width, int length)
        {
            if (_currentTheme.ObstaclePrefabs == null || _currentTheme.ObstaclePrefabs.Length == 0) return this;

            for (int i = 0; i < count; i++)
            {
                int x = (int)Random.Range(-width / 2f + 2f, width / 2f - 2f);
                int z = (int)Random.Range(-length / 2f + 2f, length / 2f - 2f);
                Vector2 pos2D = new Vector2(x, z);

                if (_usedPositions.Contains(pos2D)) continue;

                _usedPositions.Add(pos2D);
                
                // Выбираем случайный префаб из массива
                GameObject randomPrefab = _currentTheme.ObstaclePrefabs[Random.Range(0, _currentTheme.ObstaclePrefabs.Length)];
                Spawn(randomPrefab, new Vector3(x, 0.5f, z));
            }
            return this;
        }

        public ArenaData Build()
        {
            if (_arenaData == null || _arenaData.ArenaRoot == null) return null;
            return _arenaData;
        }
        
        private GameObject Spawn(GameObject prefab, Vector3 position)
        {
            GameObject obj = _container.InstantiatePrefab(prefab, position, Quaternion.identity, _arenaData.ArenaRoot);
            return obj;
        }
        
        public ArenaBuilder BuildDecorations(int count, int width, int length)
        {
            if (_currentTheme.DecorationPrefabs == null || _currentTheme.DecorationPrefabs.Length == 0) return this;

            for (int i = 0; i < count; i++)
            {
                int x = (int)Random.Range(-width / 2f + 2f, width / 2f - 2f);
                int z = (int)Random.Range(-length / 2f + 2f, length / 2f - 2f);
                Vector2 pos2D = new Vector2(x, z);

                if (_usedPositions.Contains(pos2D)) continue; // Декорации не лезут в укрытия

                _usedPositions.Add(pos2D);
                
                GameObject randomPrefab = _currentTheme.DecorationPrefabs[Random.Range(0, _currentTheme.DecorationPrefabs.Length)];
                
                // Декорациям можно дать случайный поворот для естественности
                GameObject dec = Spawn(randomPrefab, new Vector3(x, 0f, z));
                dec.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            }
            return this;
        }
        // ШАГ 5: Точки спавна врагов (Теперь с радиусом безопасности)
        public ArenaBuilder BuildEnemySpawns(int count, int width, int length)
        {
            // Безопасный радиус вокруг игрока (в метрах/клетках). 
            // Ни один враг не появится ближе, чем на этом расстоянии.
            float safeRadius = 7f; 

            for (int i = 0; i < count; i++)
            {
                int attempts = 0;
                bool found = false;
                
                while (attempts < 50 && !found)
                {
                    int x = (int)Random.Range(-width / 2f + 2f, width / 2f - 2f);
                    int z = (int)Random.Range(-length / 2f + 2f, length / 2f - 2f);
                    Vector2 pos2D = new Vector2(x, z);

                    // 1. Проверяем, свободна ли клетка
                    bool isCellFree = !_usedPositions.Contains(pos2D);
                    
                    // 2. Считаем дистанцию от этой клетки до центра арены (где стоит игрок)
                    float distanceFromPlayer = Vector2.Distance(pos2D, Vector2.zero);

                    // 3. Если клетка свободна И она находится дальше безопасного радиуса
                    if (isCellFree && distanceFromPlayer > safeRadius)
                    {
                        _usedPositions.Add(pos2D);
                        _arenaData.EnemySpawnPoints.Add(new Vector3(x, 0.5f, z));
                        found = true;
                    }
                    
                    attempts++;
                }
            }
            return this;
        }
    }
}