using UnityEngine;

namespace Input
{
    public interface IInputProvider
    {
        Vector3 GetMovementDirection();
        Vector2 GetAimPosition();
        bool IsFiring();
        
        // Добавили абстракцию для артефактов
        bool IsArtifactPressed();
        bool IsArtifactHeld();
        bool IsArtifactReleased();
    }
}