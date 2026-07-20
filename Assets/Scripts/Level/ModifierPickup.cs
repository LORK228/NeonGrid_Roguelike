using Entities.Player;
using Items;
using UI;
using UnityEngine;

namespace Level
{
    [RequireComponent(typeof(Collider))]
    public class ModifierPickup : MonoBehaviour
    {
        [SerializeField] private WeaponModifierSO _modifierData;
        
        // Для визуализации (чтобы кубик красился в цвет модификатора)
        [SerializeField] private Renderer _renderer; 

        private void Start()
        {
            // Делаем коллайдер триггером программно на всякий случай
            GetComponent<Collider>().isTrigger = true;
            
            if (_renderer != null && _modifierData != null)
            {
                _renderer.material.color = _modifierData.PickupColor;
            }
        }

        // Инициализация, если мы спавним его из кода (например, из сундука)
        public void Init(WeaponModifierSO modifierData)
        {
            _modifierData = modifierData;
            if (_renderer != null) _renderer.material.color = _modifierData.PickupColor;
        }

        private void OnTriggerEnter(Collider other)
        {
            // Проверяем, что это Игрок и у него есть пушка
            if (other.CompareTag("Player") && other.TryGetComponent(out PlayerWeapon weapon))
            {
                if (_modifierData != null)
                {
                    // ВАЖНО: Передаем ссылку на метод WrapProcessor через делегат Func
                    weapon.ApplyFireModifier(_modifierData.WrapProcessor);
                    
                    // Отправляем уведомление в глобальную шину UI
                    UIEventBus.ShowText($"СКАЧАН ФАЙЛ: {_modifierData.ModifierName.ToUpper()}");
                }

                // Уничтожаем объект после подбора (в идеале — возвращаем в пул, но для дропа сойдет и Destroy)
                Destroy(gameObject);
            }
        }
    }
}