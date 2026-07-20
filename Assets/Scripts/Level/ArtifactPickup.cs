using Entities.Components;
using Items;
using UI;
using UnityEngine;

namespace Level
{
    [RequireComponent(typeof(Collider))]
    public class ArtifactPickup : MonoBehaviour
    {
        private ArtifactSO _artifactData;
        [SerializeField] private Renderer _renderer;

        public void Init(ArtifactSO artifactData)
        {
            _artifactData = artifactData;
            // Можно менять цвет или материал на основе данных артефакта
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && other.TryGetComponent(out ArtifactHolder holder))
            {
                if (_artifactData != null)
                {
                    holder.EquipArtifact(_artifactData); // Экипируем навык
                    UIEventBus.ShowText($"ПОЛУЧЕН НАВЫК: {_artifactData.ArtifactName.ToUpper()}");
                }
                Destroy(gameObject);
            }
        }
    }
}