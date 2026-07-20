using Entities.Components;
using Time;
using Time.Interfaces;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Entities.Enemies
{
    [RequireComponent(typeof(Health), typeof(NavMeshAgent))]
    public abstract class EnemyBase : MonoBehaviour, ITimeTracker
    {
        [Header("Базовые настройки")]
        public float WakeUpDelay = 1.5f; 
        
        [Header("Оптимизация ИИ")]
        // Задержка перерасчета пути (0.2 сек = 5 раз в секунду)
        public float PathUpdateInterval = 0.2f; 

        protected IEntityTarget _target;
        protected Health _health;
        protected TimeManager _timeManager;
        protected NavMeshAgent _agent;
        
        private float _canActTime; 
        private float _lastPathUpdateTime; // Таймер для пути

        [Inject]
        public void Construct(TimeManager timeManager, IEntityTarget target)
        {
            _timeManager = timeManager;
            _target = target;
        }

        protected virtual void Awake()
        {
            _health = GetComponent<Health>();
            _agent = GetComponent<NavMeshAgent>();
            
            if (_agent != null)
                _agent.avoidancePriority = UnityEngine.Random.Range(30, 70);
        }

        protected virtual void OnEnable()
        {
            _health.OnDeath += Die;
            
            if (_timeManager != null && _timeManager.IsRewinding) _canActTime = 0f; 
            else _canActTime = UnityEngine.Time.time + WakeUpDelay;
        }

        protected virtual void OnDisable() => _health.OnDeath -= Die;

        private void Update()
        {
            // 1. Время мотается? Вырубаем агента.
            if (_timeManager != null && _timeManager.IsRewinding)
            {
                if (_agent.enabled) _agent.enabled = false;
                return;
            }
            
            // 2. Игрок мертв/пропал? Вырубаем агента.
            if (_target == null || !_target.IsActive)
            {
                if (_agent.enabled) _agent.enabled = false;
                return;
            }

            // 3. Ждем пробуждения? Вырубаем агента.
            if (UnityEngine.Time.time < _canActTime)
            {
                if (_agent.enabled) _agent.enabled = false;
                return;
            }

            // Если мы дошли сюда, значит всё окей. Включаем агента, если он был выключен.
            if (!_agent.enabled) 
            {
                _agent.enabled = true;
            }

            ExecuteBehavior();
        }

        // --- УМНЫЙ МЕТОД ДВИЖЕНИЯ ---
        protected void MoveTo(Vector3 destination)
        {
            // Обновляем путь только если прошло достаточно времени
            if (UnityEngine.Time.time >= _lastPathUpdateTime + PathUpdateInterval)
            {
                _lastPathUpdateTime = UnityEngine.Time.time;
                _agent.SetDestination(destination);
            }
        }

        protected abstract void ExecuteBehavior();
        protected virtual void Die() => gameObject.SetActive(false);
        
        
        
        // --- РЕАЛИЗАЦИЯ ИНТЕРФЕЙСА ITimeTracker ---
        public virtual void ClearHistory() { }
        public virtual void RecordState() { }
        public virtual void RewindState() { }
        
        private float _rewindStartTime;
        
        public virtual void StartRewind()
        {
            // Запоминаем, во сколько по реальному времени началась перемотка
            _rewindStartTime = UnityEngine.Time.time;
        }
        
        public virtual void StopRewind()
        {
            float rewindDuration = UnityEngine.Time.time - _rewindStartTime;
            _canActTime += rewindDuration;
        }
    }
}
