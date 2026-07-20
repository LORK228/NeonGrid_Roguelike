using Entities.Components;
using Time;
using UnityEngine;

namespace Items
{
    [CreateAssetMenu(fileName = "EMP_Blast", menuName = "NeonGrid/Items/EMP Blast")]
    public class EmpBlastArtifactSO : ArtifactSO
    {
        public float BlastRadius = 8f;
        public float Damage = 50f;
        
        [Header("Визуал")]
        public GameObject ExplosionParticlesPrefab; // Сюда можно кинуть партиклы взрыва

        public override void OnHoldStart(TimeManager timeManager, Transform origin)
        {
            // 1. Визуальный эффект (если есть префаб)
            if (ExplosionParticlesPrefab != null)
            {
                Instantiate(ExplosionParticlesPrefab, origin.position, Quaternion.identity);
            }

            // 2. Ищем всех врагов в радиусе и наносим урон
            Collider[] hits = Physics.OverlapSphere(origin.position, BlastRadius);
            foreach (var hit in hits)
            {
                if (hit.CompareTag("Enemy") && hit.TryGetComponent(out Health enemyHealth))
                {
                    enemyHealth.TakeDamage(Damage);
                }
            }
        }

        // ЭМИ бьет мгновенно, поэтому на отпускание кнопки ничего не делаем
        public override void OnHoldRelease(TimeManager timeManager, Transform origin) { } 
    }
}