using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Traps
{
    [System.Serializable]
    public struct TrapSlotSubgroup
    {
        [SerializeField] private List<TrapSlot> slots;
        [SerializeField] private Trap trap;

        public readonly IReadOnlyList<TrapSlot> Slots => slots;
        public readonly Trap Trap => trap;

        public TrapSlotSubgroup(List<TrapSlot> slots, Trap trap)
        {
            this.slots = slots;
            this.trap = trap;
        }
    }
}
