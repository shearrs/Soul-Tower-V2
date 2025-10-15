using Shears.HitDetection;
using UnityEngine;

namespace SoulTower.HitDetection
{
    public readonly struct DamageData : IHitSubdata
    {
        private readonly int damage;

        public readonly int Damage => damage;

        public DamageData(int damage)
        {
            this.damage = damage;
        }
    }
}
