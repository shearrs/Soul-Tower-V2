using Shears.Signals;
using UnityEngine;

namespace SoulTower.Enemies
{
    public readonly struct SetDefenderShieldsSignal : ISignal
    {
        private readonly DefenderShield.Direction direction;

        public readonly DefenderShield.Direction Direction => direction;

        public SetDefenderShieldsSignal(DefenderShield.Direction direction)
        {
            this.direction = direction;
        }
    }
}
