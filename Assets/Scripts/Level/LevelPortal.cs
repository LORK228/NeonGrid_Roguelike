using System;
using UnityEngine;

namespace Level
{
    [RequireComponent(typeof(Collider))]
    public class LevelPortal : MonoBehaviour
    {
        public static event Action OnPortalEntered;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                OnPortalEntered?.Invoke();
                Destroy(gameObject); // Уничтожаем сам портал
            }
        }
    }
}