using Items;
using Time;
using UnityEngine;
using Zenject;

namespace Entities.Components
{
    public class ArtifactHolder : MonoBehaviour
    {
        [SerializeField] private ArtifactSO _activeArtifact;
        public ArtifactSO ActiveArtifact => _activeArtifact;

        // Holder берет ответственность за получение зависимостей на себя
        private TimeManager _timeManager;

        [Inject]
        public void Construct(TimeManager timeManager)
        {
            _timeManager = timeManager;
        }

        public void EquipArtifact(ArtifactSO newArtifact)
        {
            _activeArtifact = newArtifact;
            // Убрали грязный хак с _container.Inject()
        }

        // Добавляем удобные методы-обертки
        public void UseArtifactStart() => _activeArtifact?.OnHoldStart(_timeManager, transform);
        public void UseArtifactRelease() => _activeArtifact?.OnHoldRelease(_timeManager, transform);
    }
}