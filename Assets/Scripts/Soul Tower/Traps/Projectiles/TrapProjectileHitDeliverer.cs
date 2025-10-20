using Shears.HitDetection;
using SoulTower.HitDetection;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Traps
{
    public class TrapProjectileHitDeliverer : HitDeliverer3D
    {
        [SerializeField] private TrapData data;

        private readonly List<DamageData> damageData = new();

        [ContextMenu("Update Damage Data")]
        private void UpdateDamageData()
        {
            damageData.Clear();

            foreach (var data in data.DamageData)
                damageData.Add(data);
        }

        private void Awake()
        {
            UpdateDamageData();
        }

        public void AddData(DamageData data)
        {
            damageData.Add(data);
        }

        public override IReadOnlyCollection<IHitSubdata> GetCustomData()
        {
            return damageData;
        }
    }
}
