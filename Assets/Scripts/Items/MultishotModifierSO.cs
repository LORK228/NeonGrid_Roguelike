using Combat.Modifiers;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "Mod_Multishot", menuName = "NeonGrid/Modifiers/Multishot")]
    public class MultishotModifierSO : WeaponModifierSO
    {
        [Header("Настройки мультишота")]
        public float SpreadAngle = 15f;

        public override IFireProcessor WrapProcessor(IFireProcessor currentProcessor)
        {
            return new MultishotDecorator(currentProcessor, SpreadAngle);
        }
    }
}