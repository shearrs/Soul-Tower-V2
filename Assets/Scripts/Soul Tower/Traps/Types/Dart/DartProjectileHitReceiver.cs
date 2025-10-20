using Shears.HitDetection;
using SoulTower.HitDetection;
using UnityEngine;

namespace SoulTower.Traps
{
    public class DartProjectileHitReceiver : HitReceiver3D
    {
        [SerializeField] private DartProjectile dart;

        protected override void ReceiveHit(HitData3D hitData)
        {
            if (dart.OnFire)
                return;

            foreach (var data in hitData.Data)
            {
                if (data is DamageData damageData)
                {
                    if (damageData.Type != DamageType.Fire)
                        continue;

                    dart.LightOnFire();
                }
            }
        }
    }
}
