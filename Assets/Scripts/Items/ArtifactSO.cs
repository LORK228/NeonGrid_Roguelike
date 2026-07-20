using Time;
using UnityEngine;

namespace Items
{
    public abstract class ArtifactSO : ScriptableObject
    {
        public string ArtifactName;
        [TextArea] public string Description;
        public Sprite Icon;

        // Добавили Transform origin (откуда кастуем)
        public abstract void OnHoldStart(TimeManager timeManager, Transform origin);
        public abstract void OnHoldRelease(TimeManager timeManager, Transform origin);
    }
}