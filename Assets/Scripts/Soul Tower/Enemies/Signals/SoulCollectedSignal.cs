using Shears.Signals;
using UnityEngine;

namespace SoulTower.Enemies
{
    public readonly struct SoulCollectedSignal : ISignal
    {
        private readonly DroppedSoul droppedSoul;

        public readonly DroppedSoul DroppedSoul => droppedSoul;

        public SoulCollectedSignal(DroppedSoul droppedSoul)
        {
            this.droppedSoul = droppedSoul;
        }
    }
}
