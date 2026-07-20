using UnityEngine;
using Zenject;
using Combat.Modifiers;
using Combat.Weapons;
using Core.Pool;
using Time;

namespace Entities.Player
{
    public class PlayerWeapon : MonoBehaviour
    {
        [Header("Экипировка")]
        [SerializeField] private WeaponStrategySO _currentWeapon;
        [SerializeField] private Transform _firePoint;

        private float _lastFireTime;
        private PoolManager _poolManager;
        private TimeManager _timeManager; // Добавляем ссылку на время
        
        private IFireProcessor _fireProcessor;
        private BaseFireProcessor _baseFireProcessor;

        [Inject]
        public void Construct(PoolManager poolManager, TimeManager timeManager, BaseFireProcessor baseFireProcessor)
        {
            _poolManager = poolManager;
            _timeManager = timeManager; // Инжектим!
            _baseFireProcessor = baseFireProcessor;
        }

        private void Start()
        {
            ResetWeaponModifiers();
        }

        public GameObject TryFire()
        {
            if (_currentWeapon == null || _firePoint == null || _poolManager == null || _fireProcessor == null) 
                return null;

            if (UnityEngine.Time.time >= _lastFireTime + _currentWeapon.Cooldown)
            {
                _lastFireTime = UnityEngine.Time.time;
                
                // ВАЖНО: Вызываем процессор с новыми аргументами.
                // isEnemy передаем как false, так как стреляет игрок.
                return _fireProcessor.ExecuteFire(
                    _firePoint.position, 
                    _firePoint.rotation, 
                    _poolManager, 
                    _timeManager, 
                    _currentWeapon, 
                    false
                );
            }
            
            return null;
        }

        public void ApplyFireModifier(System.Func<IFireProcessor, IFireProcessor> decoratorFactory)
        {
            _fireProcessor = decoratorFactory(_fireProcessor);
        }

        public void ResetWeaponModifiers()
        {
            _fireProcessor = _baseFireProcessor;
        }
    }
}   