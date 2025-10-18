using Shears.HitDetection;
using Shears.Logging;
using SoulTower.HitDetection;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Traps
{
    public class TrapHitDeliverer : HitDeliverer3D
    {
        private readonly List<DamageData> damageData = new();
        private Trap trap;

        [ContextMenu("Update Damage Data")]
        private void UpdateDamageData()
        {
            damageData.Clear();

            foreach (var data in trap.DamageData)
                damageData.Add(data);
        }

        private void Awake()
        {
            if (!TryGetComponent(out trap))
                trap = GetComponentInParent<Trap>();

            if (trap == null)
            {
                SHLogger.Log($"{nameof(TrapHitDeliverer)} needs to be attached or a child of a trap!");
                return;
            }

            UpdateDamageData();
        }

        public override IReadOnlyCollection<IHitSubdata> GetCustomData()
        {
            return damageData;
        }
    }
}
