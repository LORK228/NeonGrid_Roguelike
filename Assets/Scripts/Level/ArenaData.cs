using System.Collections.Generic;
using UnityEngine;

namespace Level
{
    // DTO (Data Transfer Object) - чистый контейнер данных без логики
    public class ArenaData
    {
        public Transform ArenaRoot;
        public Vector3 PlayerSpawnPoint;
        public List<Vector3> EnemySpawnPoints = new List<Vector3>();
    }
}