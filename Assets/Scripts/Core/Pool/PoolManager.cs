using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Core.Pool
{
    [System.Serializable]
    public class PoolSetup
    {
        public GameObject Prefab;
        public int InitialSize = 20;
    }

    public class PoolManager : IInitializable
    {
        private readonly List<PoolSetup> _setups;
        private readonly DiContainer _container;
        
        // Словари для хранения очередей и папок
        private readonly Dictionary<GameObject, Queue<GameObject>> _poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();
        private readonly Dictionary<GameObject, Transform> _poolFolders = new Dictionary<GameObject, Transform>();
        
        private readonly Transform _poolRoot;

        public PoolManager(List<PoolSetup> setups, DiContainer container)
        {
            _setups = setups;
            _container = container;
            _poolRoot = new GameObject("[PoolRoot]").transform;
        }

        public void Initialize() 
        {
            foreach (var setup in _setups)
            {
                if (setup.Prefab == null) continue;
                CreatePool(setup.Prefab, setup.InitialSize);
            }
        }

        // Вынес логику создания пула в отдельный метод, чтобы избежать дублирования
        private void CreatePool(GameObject prefab, int size)
        {
            if (_poolDictionary.ContainsKey(prefab)) return;

            _poolDictionary.Add(prefab, new Queue<GameObject>());
            
            // Создаем папку для этого конкретного префаба
            GameObject folder = new GameObject($"[{prefab.name}s]");
            folder.transform.SetParent(_poolRoot);
            _poolFolders.Add(prefab, folder.transform);

            for (int i = 0; i < size; i++)
            {
                GameObject newObj = SpawnNew(prefab, Vector3.zero, Quaternion.identity, prefab);
                newObj.SetActive(false);
                _poolDictionary[prefab].Enqueue(newObj);
            }
        }

        public GameObject SpawnFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (!_poolDictionary.ContainsKey(prefab))
            {
                CreatePool(prefab, 5);
            }

            Queue<GameObject> queue = _poolDictionary[prefab];

            while (queue.Count > 0)
            {
                GameObject objectToSpawn = queue.Dequeue();
        
                if (objectToSpawn == null) continue; 

                // FAIL-FAST: Бросаем исключение! Это критическая архитектурная ошибка.
                if (objectToSpawn.activeSelf)
                {
                    throw new System.InvalidOperationException(
                        $"[PoolManager] КРИТИЧЕСКАЯ ОШИБКА: Объект {objectToSpawn.name} активен, " +
                        "находясь в очереди пула! Проверьте двойные вызовы Return() или ручные SetActive(true).");
                }

                objectToSpawn.transform.SetPositionAndRotation(position, rotation);
                objectToSpawn.SetActive(true); 
    
                return objectToSpawn;
            }

            // Если пул опустел — создаем новый объект
            GameObject newObj = SpawnNew(prefab, position, rotation, prefab);
            newObj.SetActive(true); 
            return newObj;
        }

        public void ReturnToPool(GameObject obj, GameObject poolKey)
        {
            if (!obj.activeSelf)
            {
                // Защита от двойного возврата (Double-free)
                throw new System.InvalidOperationException(
                    $"[PoolManager] Объект {obj.name} уже деактивирован! " +
                    "Возможен двойной вызов Return().");
            }

            obj.SetActive(false);
    
            if (_poolFolders.TryGetValue(poolKey, out Transform folder))
            {
                obj.transform.SetParent(folder);
            }
    
            _poolDictionary[poolKey].Enqueue(obj);
        }

        private GameObject SpawnNew(GameObject prefab, Vector3 position, Quaternion rotation, GameObject poolKey)
        {
            // Определяем, в какую папку положить новый объект
            Transform parentFolder = _poolFolders.ContainsKey(poolKey) ? _poolFolders[poolKey] : _poolRoot;
            
            // Сразу спавним объект внутрь его папки
            GameObject newObj = _container.InstantiatePrefab(prefab, position, rotation, parentFolder);
            
            if (!newObj.TryGetComponent(out PooledObject pooledObj))
            {
                pooledObj = newObj.AddComponent<PooledObject>();
            }
            
            pooledObj.PoolKey = poolKey;
            pooledObj.Manager = this;
            return newObj;
        }
    }

    public class PooledObject : MonoBehaviour
    {
        public GameObject PoolKey;
        public PoolManager Manager;
        public void Return() => Manager.ReturnToPool(gameObject, PoolKey);
    }
}