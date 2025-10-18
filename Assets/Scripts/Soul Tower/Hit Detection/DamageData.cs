using Shears.HitDetection;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.HitDetection
{
    [System.Serializable]
    public class DamageData : IHitSubdata
    {
        [SerializeField] private DamageType type;
        [SerializeField] private int damage;
        [SerializeReference] private IStatus[] statuses;

        public int Damage => damage;
        public DamageType Type => type;
        public IReadOnlyCollection<IStatus> Statuses => statuses;

        public DamageData(int damage, DamageType type)
        {
            this.damage = damage;
            this.type = type;
        }
    }
}
