using UnityEngine;

namespace StateMachine
{
    [System.Serializable]
    public class GameStateSettings
    {
        [Header("Префабы для стадий игры")]
        public GameObject RewardChestPrefab;
        public GameObject LevelPortalPrefab;
    }
}