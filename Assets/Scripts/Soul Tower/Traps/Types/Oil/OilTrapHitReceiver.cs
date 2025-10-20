using Shears.HitDetection;
using SoulTower.HitDetection;
using UnityEngine;

namespace SoulTower.Traps
{
    public class OilTrapHitReceiver : HitReceiver3D
    {
        [SerializeField] private OilTrap trap;

        protected override void ReceiveHit(HitData3D hitData)
        {
            if (trap.IsOnFire)
                return;

            foreach (var data in hitData.Data)
            {
                if (data is DamageData damageData)
                {
                    if (damageData.Type == DamageType.Fire)
                    {
                        trap.LightOnFire();

                        return;
                    }
                }
            }
        }
    }
}
