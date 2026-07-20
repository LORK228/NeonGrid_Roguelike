using Entities.Components;
using Time.Interfaces;
using UnityEngine;

namespace Time.Trackers
{
    public class HealthTimeTracker : MonoBehaviour, ITimeTracker
    {
        public float RecordTime = 5f;
        
        private TimeRewindBuffer<float> _buffer;
        private Health _health;

        private void Awake() 
        {
            _health = GetComponent<Health>();
            int capacity = Mathf.RoundToInt(RecordTime / UnityEngine.Time.fixedDeltaTime);
            _buffer = new TimeRewindBuffer<float>(capacity);
        }

        public void ClearHistory() => _buffer.Clear();
        public void StartRewind() { } 
        public void StopRewind() { }

        public void RecordState()
        {
            if (_health != null)
                _buffer.Record(_health.CurrentHealth);
        }

        public void RewindState()
        {
            if (_health != null && _buffer.TryRewind(out float previousHealth))
            {
                _health.RestoreHealthForTimeRewind(previousHealth);
            }
        }
    }
}