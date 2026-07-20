using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
    public class DesktopInputProvider : IInputProvider
    {
        // Идеально было бы использовать InputActionAsset, 
        // но пока оставляем опросы напрямую без хардкода внутри контроллера.
    
        public Vector3 GetMovementDirection()
        {
            if (Keyboard.current == null) return Vector3.zero;
        
            float h = (Keyboard.current.dKey.isPressed ? 1f : 0f) - (Keyboard.current.aKey.isPressed ? 1f : 0f);
            float v = (Keyboard.current.wKey.isPressed ? 1f : 0f) - (Keyboard.current.sKey.isPressed ? 1f : 0f);
            return new Vector3(h, 0f, v).normalized;
        }

        public Vector2 GetAimPosition() => Mouse.current?.position.ReadValue() ?? Vector2.zero;
        public bool IsFiring() => Mouse.current != null && Mouse.current.leftButton.isPressed;
    
        // Поддержка артефактов
        public bool IsArtifactPressed() => Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
        public bool IsArtifactHeld() => Keyboard.current != null && Keyboard.current.spaceKey.isPressed;
        public bool IsArtifactReleased() => Keyboard.current != null && Keyboard.current.spaceKey.wasReleasedThisFrame;
    }
}