using System;
using Items;
using UnityEngine;

namespace Level
{
    [RequireComponent(typeof(Collider))]
    public class RewardChest : MonoBehaviour
    {
        public static event Action<Vector3> OnChestOpened;

        [Header("Модификаторы пушки (Пассивки)")]
        [SerializeField] private ModifierPickup _modifierPickupPrefab;
        [SerializeField] private WeaponModifierSO[] _possibleModifiers;

        [Header("Артефакты (Активные навыки)")]
        [SerializeField] private ArtifactPickup _artifactPickupPrefab;
        [SerializeField] private ArtifactSO[] _possibleArtifacts;

        [Header("Шансы")]
        [Range(0f, 1f)] [SerializeField] private float _artifactDropChance = 0.3f; // 30% шанс на активный навык

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Vector3 dropPos = transform.position + new Vector3(-1.5f, 0, 0);

                // Решаем, что выпадет
                if (UnityEngine.Random.value < _artifactDropChance && _possibleArtifacts.Length > 0)
                {
                    // Выпадает Артефакт (например, Перемотка времени)
                    ArtifactSO randomArtifact = _possibleArtifacts[UnityEngine.Random.Range(0, _possibleArtifacts.Length)];
                    ArtifactPickup drop = Instantiate(_artifactPickupPrefab, dropPos, Quaternion.identity);
                    drop.Init(randomArtifact);
                }
                else if (_possibleModifiers.Length > 0)
                {
                    // Выпадает Модификатор пушки
                    WeaponModifierSO randomMod = _possibleModifiers[UnityEngine.Random.Range(0, _possibleModifiers.Length)];
                    ModifierPickup drop = Instantiate(_modifierPickupPrefab, dropPos, Quaternion.identity);
                    drop.Init(randomMod);
                }

                OnChestOpened?.Invoke(transform.position);
                Destroy(gameObject);
            }
        }
    }
}