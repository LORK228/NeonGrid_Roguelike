using Combat.Modifiers;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "Mod_RearShot", menuName = "NeonGrid/Modifiers/Rear Shot")]
    public class RearShotModifierSO : WeaponModifierSO
    {
        public override IFireProcessor WrapProcessor(IFireProcessor currentProcessor) => new RearShotDecorator(currentProcessor);
    }
}