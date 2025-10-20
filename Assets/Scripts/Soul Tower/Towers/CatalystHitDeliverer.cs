using Shears.HitDetection;
using SoulTower.HitDetection;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Towers
{
    public class CatalystHitDeliverer : HitDeliverer3D
    {
        private readonly List<DamageData> damageData = new() { new(10, DamageType.None) };

        public override IReadOnlyCollection<IHitSubdata> GetCustomData()
        {
            return damageData;
        }
    }
}
