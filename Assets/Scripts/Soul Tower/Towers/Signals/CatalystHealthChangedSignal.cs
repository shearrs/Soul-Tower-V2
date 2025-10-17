using Shears.Signals;
using UnityEngine;

namespace SoulTower.Towers
{
    public readonly struct CatalystHealthChangedSignal : ISignal
    {
        private readonly int health;

        public readonly int Health => health;

        public CatalystHealthChangedSignal(int health)
        {
            this.health = health;
        }
    }
}
