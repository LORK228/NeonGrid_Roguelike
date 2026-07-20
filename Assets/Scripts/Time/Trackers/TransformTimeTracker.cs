using Time.Interfaces;
using UnityEngine;

namespace Time.Trackers
{
    public class TransformTimeTracker : MonoBehaviour, ITimeTracker
    {
        public float RecordTime = 5f;
        
        private TimeRewindBuffer<PointInTime> _buffer;
        private Rigidbody _rb;
        private Vector3 _lastVelocity;

        private struct PointInTime 
        { 
            public Vector3 Pos; 
            public Quaternion Rot; 
            public Vector3 Vel; 
        }

        private void Awake() 
        {
            _rb = GetComponent<Rigidbody>();
            
            // Вычисляем размер массива один раз на старте
            int capacity = Mathf.RoundToInt(RecordTime / UnityEngine.Time.fixedDeltaTime);
            _buffer = new TimeRewindBuffer<PointInTime>(capacity);
        }

        public void ClearHistory() => _buffer.Clear();

        public void StartRewind() 
        { 
            if (_rb != null && gameObject.activeInHierarchy) 
            {
                _rb.isKinematic = true;
            }
        }
        
        public void StopRewind() 
        { 
            if (_rb != null && gameObject.activeInHierarchy) 
            { 
                _rb.isKinematic = false; 
                _rb.linearVelocity = _lastVelocity; 
            } 
        }

        public void RecordState()
        {
            _buffer.Record(new PointInTime 
            { 
                Pos = transform.position, 
                Rot = transform.rotation, 
                Vel = _rb != null ? _rb.linearVelocity : Vector3.zero 
            });
        }

        public void RewindState()
        {
            if (_buffer.TryRewind(out PointInTime snap))
            {
                transform.SetPositionAndRotation(snap.Pos, snap.Rot);
                _lastVelocity = snap.Vel;
            }
        }
    }
}