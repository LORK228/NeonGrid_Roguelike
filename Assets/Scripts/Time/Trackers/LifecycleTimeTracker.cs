using Core.Pool;
using Time.Interfaces;
using UnityEngine;

namespace Time.Trackers
{
    public class LifecycleTimeTracker : MonoBehaviour, ITimeTracker
    {
        [Header("Настройки истории")]
        public float RecordTime = 5f;
        public bool DestroyOnHistoryEnd = false;

        private TimeRewindBuffer<bool> _buffer;
        private bool _hasBirthMemory = true;

        private void Awake()
        {
            int capacity = Mathf.RoundToInt(RecordTime / UnityEngine.Time.fixedDeltaTime);
            _buffer = new TimeRewindBuffer<bool>(capacity);
        }

        public void ClearHistory()
        {
            _buffer.Clear();
            _hasBirthMemory = true;
        }

        public void StartRewind() { } 
        public void StopRewind() { }

        public void RecordState()
        {
            // Если буфер вот-вот заполнится полностью, мы стираем память о рождении
            if (_buffer.Count >= _buffer.Capacity - 1)
            {
                _hasBirthMemory = false;
            }

            _buffer.Record(gameObject.activeSelf);
        }

        public void RewindState()
        {
            if (_buffer.TryRewind(out bool wasActive))
            {
                if (gameObject.activeSelf != wasActive)
                {
                    gameObject.SetActive(wasActive);
                }
            }
            else
            {
                // Защита: возвращаем в пул, ТОЛЬКО если объект прямо сейчас активен на сцене
                if (DestroyOnHistoryEnd && _hasBirthMemory && gameObject.activeSelf)
                {
                    if (TryGetComponent(out PooledObject po)) 
                    {
                        po.Return();
                    }
                    else 
                    {
                        gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}