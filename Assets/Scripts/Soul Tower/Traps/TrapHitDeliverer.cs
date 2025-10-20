using Shears.HitDetection;
using SoulTower.HitDetection;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Traps
{
    [RequireComponent(typeof(Trap))]
    public class TrapHitDeliverer : HitDeliverer3D
    {
        private Trap trap;

        private readonly List<DamageData> damageData = new();

        [ContextMenu("Update Damage Data")]
        private void UpdateDamageData()
        {
            damageData.Clear();

            foreach (var data in trap.DamageData)
                damageData.Add(data);
        }

        private void Awake()
        {
            trap = GetComponent<Trap>();

            UpdateDamageData();
        }

        public override IReadOnlyCollection<IHitSubdata> GetCustomData()
        {
            return damageData;
        }
    }
}
