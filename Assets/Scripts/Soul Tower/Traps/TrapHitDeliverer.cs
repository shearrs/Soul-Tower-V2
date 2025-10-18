using Shears.HitDetection;
using SoulTower.HitDetection;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Traps
{
    public class TrapHitDeliverer : HitDeliverer3D
    {
        [SerializeField] private TrapData trapData;

        private readonly List<DamageData> damageData = new();

        [ContextMenu("Update Damage Data")]
        private void UpdateDamageData()
        {
            damageData.Clear();

            foreach (var data in trapData.DamageData)
                damageData.Add(data);
        }

        private void Awake()
        {
            UpdateDamageData();
        }

        public override IReadOnlyCollection<IHitSubdata> GetCustomData()
        {
            return damageData;
        }
    }
}
