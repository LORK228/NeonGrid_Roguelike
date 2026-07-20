using UnityEngine;

namespace Input
{
    public class MobileInputProvider : IInputProvider
    {
        // Требуется инициализация через конструктор или Inject!
        private readonly Joystick _moveJoystick; 
        private readonly Joystick _aimJoystick;

        public MobileInputProvider(Joystick moveJoystick, Joystick aimJoystick)
        {
            _moveJoystick = moveJoystick;
            _aimJoystick = aimJoystick;
        }

        public Vector3 GetMovementDirection() => _moveJoystick != null ? new Vector3(_moveJoystick.Horizontal, 0, _moveJoystick.Vertical).normalized : Vector3.zero;
        public Vector2 GetAimPosition() => _aimJoystick != null ? new Vector2(_aimJoystick.Horizontal, _aimJoystick.Vertical) : Vector2.zero;
    
        // Заглушки, которые нужно будет привязать к UI-кнопкам
        public bool IsFiring() => _aimJoystick != null && (_aimJoystick.Horizontal != 0 || _aimJoystick.Vertical != 0);
        public bool IsArtifactPressed() => false; 
        public bool IsArtifactHeld() => false;
        public bool IsArtifactReleased() => false;
    }
}