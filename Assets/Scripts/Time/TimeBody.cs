using Time.Interfaces;
using UnityEngine;
using Zenject;

namespace Time
{
    public class TimeBody : MonoBehaviour, ITimeTrackable
    {
        private ITimeTracker[] _trackers;
        private TimeManager _timeManager;

        [Inject]
        public void Construct(TimeManager timeManager) => _timeManager = timeManager;

        private void Awake() 
        {
            // Находим все узкоспециализированные трекеры на этом же объекте
            _trackers = GetComponents<ITimeTracker>();
        }

        private void Start() { if (_timeManager != null) _timeManager.Register(this); }
        private void OnDestroy() { if (_timeManager != null) _timeManager.Unregister(this); }

        public void ClearHistory() { foreach (var t in _trackers) t.ClearHistory(); }
        public void StartRewind() { foreach (var t in _trackers) t.StartRewind(); }
        public void StopRewind() { foreach (var t in _trackers) t.StopRewind(); }
        public void RecordState() { foreach (var t in _trackers) t.RecordState(); }
        public void RewindState() { foreach (var t in _trackers) t.RewindState(); }
    }
}