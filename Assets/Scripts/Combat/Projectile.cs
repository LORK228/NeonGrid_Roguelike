using UnityEngine;
using Core.Pool;
using Time;
using Entities.Components;

namespace Combat
{
    public class Projectile : MonoBehaviour
    {
        [Header("Настройки")]
        [SerializeField] private float _baseLifeTime = 2f;
        
        public float Damage { get; private set; }
        public bool IsEnemyProjectile { get; private set; } 
        
        private float _currentLifeTime;
        private TimeManager _timeManager;
        
        // --- ДЛЯ ПРАВИЛЬНОГО МАСШТАБА ---
        private Vector3 _originalScale;

        private void Awake()
        {
            // Запоминаем изначальный размер префаба один раз при рождении
            _originalScale = transform.localScale;
        }

        public void Init(TimeManager timeManager, float damage, bool isEnemy)
        {
            _timeManager = timeManager;
            Damage = damage;
            IsEnemyProjectile = isEnemy;
            _currentLifeTime = _baseLifeTime;
        }

        private void Update()
        {
            if (_timeManager != null && _timeManager.IsRewinding) return;
            
            _currentLifeTime -= UnityEngine.Time.deltaTime;
            if (_currentLifeTime <= 0f) Deactivate();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out Projectile _)) return;
            
            if (!IsEnemyProjectile && other.CompareTag("Player")) return;
            if (IsEnemyProjectile && other.CompareTag("Enemy")) return;

            // Игнорируем пол, чтобы огромные пули не взрывались об него при спавне!
            // (Убедись, что твой префаб пола имеет тег "Floor" или просто игнорируй коллайдеры без компонентов)
            if (other.CompareTag("Floor")) return;

            if (other.TryGetComponent(out Health targetHealth))
            {
                targetHealth.TakeDamage(Damage);
            }

            Deactivate();
        }

        private void Deactivate()
        {
            if (!gameObject.activeSelf) return;

            if (TryGetComponent(out PooledObject po)) po.Return();
            else gameObject.SetActive(false);
        }
        
        // --- НОВЫЕ МЕТОДЫ ДЛЯ ДЕКОРАТОРОВ ---
        public void MultiplyDamage(float multiplier)
        {
            Damage *= multiplier;
        }

        public void MultiplyScale(float multiplier)
        {
            // Умножаем ИЗНАЧАЛЬНЫЙ масштаб, а не текущий, чтобы избежать багов
            transform.localScale = _originalScale * multiplier;
        }

        private void OnDisable()
        {
            // ОЧИСТКА ПУЛА: Возвращаем пуле нормальный размер, когда она прячется в пул!
            transform.localScale = _originalScale;
        }
    }
}