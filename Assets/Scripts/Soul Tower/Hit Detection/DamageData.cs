using Shears.HitDetection;
using UnityEngine;

namespace SoulTower.HitDetection
{
    [System.Serializable]
    public class DamageData : IHitSubdata
    {
        [SerializeField] private DamageType type;
        [SerializeField] private int damage;

        public int Damage => damage;
        public DamageType Type => type;

        public DamageData(int damage, DamageType type)
        {
            this.damage = damage;
            this.type = type;
        }
    }
}
