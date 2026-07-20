using Time.Interfaces;
using UnityEngine;

namespace Time.Trackers
{
    [RequireComponent(typeof(CharacterController))]
    public class CharacterControllerTimeTracker : MonoBehaviour, ITimeTracker
    {
        public float RecordTime = 5f;
        
        private TimeRewindBuffer<PointInTime> _buffer;
        private CharacterController _characterController;
        
        // Переменные для точной линейной интерполяции (Zero-Jitter)
        private PointInTime _pointFrom;
        private PointInTime _pointTo;
        private float _interpolationTimer;
        private bool _isRewinding;

        private struct PointInTime 
        { 
            public Vector3 Pos; 
            public Quaternion Rot; 
        }

        private void Awake() 
        {
            _characterController = GetComponent<CharacterController>();
            int capacity = Mathf.RoundToInt(RecordTime / UnityEngine.Time.fixedDeltaTime);
            _buffer = new TimeRewindBuffer<PointInTime>(capacity);
        }

        public void ClearHistory() => _buffer.Clear();

        public void StartRewind() 
        { 
            if (_characterController != null) _characterController.enabled = false;
            _isRewinding = true;
            
            // Захватываем текущее положение как стартовую точку
            _pointFrom = new PointInTime { Pos = transform.position, Rot = transform.rotation };
            _pointTo = _pointFrom;
            _interpolationTimer = 1f; // Ставим 1, чтобы стоять на месте до первого тика FixedUpdate
        }
        
        public void StopRewind() 
        { 
            transform.SetPositionAndRotation(_pointTo.Pos, _pointTo.Rot);
            Physics.SyncTransforms(); 
            
            if (_characterController != null) _characterController.enabled = true; 
            _isRewinding = false;
        }

        public void RecordState()
        {
            _buffer.Record(new PointInTime 
            { 
                Pos = transform.position, 
                Rot = transform.rotation 
            });
        }

        // Вызывается в FixedUpdate (стандартно 50 раз в секунду)
        public void RewindState() 
        {
            if (_buffer.TryRewind(out PointInTime snap))
            {
                // Текущая цель становится отправной точкой
                _pointFrom = _pointTo;
                // Снимок из прошлого становится новой целью
                _pointTo = snap;
                
                // Сбрасываем таймер для Update
                _interpolationTimer = 0f;
            }
        }

        // Вызывается каждый кадр отрисовки экрана (например, 144 раза в секунду)
        private void Update() 
        {
            if (!_isRewinding) return;

            // Вычисляем процент времени, прошедшего между двумя физическими кадрами.
            // Если игра работает в 100 FPS, а физика в 50 FPS, за один Update прибавится ровно 0.5.
            _interpolationTimer += UnityEngine.Time.deltaTime / UnityEngine.Time.fixedDeltaTime;
            
            // Защита от переполнения (если Update вызвался больше раз, чем ожидалось)
            float t = Mathf.Clamp01(_interpolationTimer);

            // Идеально ровное движение от кадра A к кадру B
            transform.position = Vector3.Lerp(_pointFrom.Pos, _pointTo.Pos, t);
            transform.rotation = Quaternion.Slerp(_pointFrom.Rot, _pointTo.Rot, t);
        }
    }
}