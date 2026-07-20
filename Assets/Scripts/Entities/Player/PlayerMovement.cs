using Entities.Components;
using UnityEngine;

namespace Entities.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour, IEntityTarget
    {
        public Vector3 Position => transform.position;
        public bool IsActive => gameObject.activeInHierarchy;
        public GameObject TargetObject => gameObject;
        
        [Header("Настройки")]
        [SerializeField] private float MoveSpeed = 8f;
        
        private CharacterController _characterController;
        private Camera _mainCamera;
        private Plane _groundPlane; // Математическая плоскость пола
        
        
        
        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
            _mainCamera = Camera.main;
            // Создаем невидимую плоскость на высоте Y = 0, смотрящую вверх
            _groundPlane = new Plane(Vector3.up, Vector3.zero); 
        }

        // Вызывается Командой
        public void Move(Vector3 direction)
        {
            // Двигаем контроллер с учетом Time.deltaTime внутри самого метода Move
            Vector3 motion = direction * (MoveSpeed * UnityEngine.Time.deltaTime);
            _characterController.Move(motion);
        }

        public void AimTowardsMouse(Vector2 screenMousePos)
        {
            if (_mainCamera == null) return; // <-- Защита от падения

            Ray ray = _mainCamera.ScreenPointToRay(screenMousePos);
            
            if (_groundPlane.Raycast(ray, out float rayDistance))
            {
                Vector3 lookPoint = ray.GetPoint(rayDistance);
                Vector3 lookDirection = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
                
                transform.LookAt(lookDirection);
            }
        }
        
        public void WarpToPosition(Vector3 newPosition)
        {
            _characterController.enabled = false; // Выключаем физику
            transform.position = newPosition;     // Телепортируем
            _characterController.enabled = true;  // Включаем физику
        }
    }
}