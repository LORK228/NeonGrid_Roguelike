using Combat.Modifiers;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "Mod_HeavyDamage", menuName = "NeonGrid/Modifiers/Heavy Damage")]
    public class HeavyDamageModifierSO : WeaponModifierSO
    {
        [Header("Настройки урона")]
        public float DamageMultiplier = 2f; // Появится в инспекторе!

        public override IFireProcessor WrapProcessor(IFireProcessor currentProcessor)
        {
            return new HeavyDamageDecorator(currentProcessor, DamageMultiplier);
        }
    }
}