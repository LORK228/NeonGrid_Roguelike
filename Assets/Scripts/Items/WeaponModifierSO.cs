using Combat.Modifiers;
using UnityEngine;

namespace Items
{
    public abstract class WeaponModifierSO : ScriptableObject
    {
        [Header("Визуал и UI")]
        public string ModifierName;
        public Color PickupColor = Color.cyan;

        // Абстрактный метод, который обязуется обернуть текущий процессор в новый декоратор
        public abstract IFireProcessor WrapProcessor(IFireProcessor currentProcessor);
    }
}