using UnityEngine;

namespace Entities.Components
{
    public interface IEntityTarget
    {
        Vector3 Position { get; }
        bool IsActive { get; }
        GameObject TargetObject { get; } // Добавили эту строку
    }
}