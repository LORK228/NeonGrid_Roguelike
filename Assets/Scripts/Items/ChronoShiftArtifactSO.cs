using Time;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "New Chrono Artifact", menuName = "NeonGrid/Items/ChronoShift")]
    public class ChronoShiftArtifactSO : ArtifactSO
    {
        // Никаких [Inject]! SO абсолютно чист и независим.
        public override void OnHoldStart(TimeManager timeManager, Transform origin) { if (timeManager != null) timeManager.StartRewind(); }
        public override void OnHoldRelease(TimeManager timeManager, Transform origin) { if (timeManager != null) timeManager.StopRewind(); }
    }
}