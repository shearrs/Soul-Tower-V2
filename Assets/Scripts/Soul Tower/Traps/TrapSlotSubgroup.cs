using Shears;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Traps
{
    [System.Serializable]
    public struct TrapSlotSubgroup
    {
        [SerializeField, ReadOnly] private bool isMultigroup;
        [SerializeField] private List<TrapSlot> slots;
        [SerializeField] private Trap trap;
        [SerializeField] private List<Trap> traps;

        public readonly bool IsMultigroup => isMultigroup;
        public readonly IReadOnlyList<TrapSlot> Slots => slots;
        public readonly IReadOnlyList<Trap> Traps => traps;
        public readonly Trap Trap => trap;

        public TrapSlotSubgroup(List<TrapSlot> slots, Trap trap)
        {
            isMultigroup = false;
            this.slots = slots;
            this.trap = trap;
            traps = null;
        }

        public TrapSlotSubgroup(List<TrapSlot> slots, List<Trap> traps)
        {
            isMultigroup = true;
            this.slots = slots;
            this.traps = traps;
            trap = null;
        }
    }
}
