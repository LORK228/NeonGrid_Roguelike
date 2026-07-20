using Combat.Modifiers;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "Mod_GiantBullet", menuName = "NeonGrid/Modifiers/Giant Bullet")]
    public class GiantBulletModifierSO : WeaponModifierSO
    {
        [Header("Настройки размера")]
        public float ScaleMultiplier = 2f;
        public float DamageMultiplier = 1.5f;

        public override IFireProcessor WrapProcessor(IFireProcessor currentProcessor)
        {
            return new GiantBulletDecorator(currentProcessor, ScaleMultiplier, DamageMultiplier);
        }
    }
}