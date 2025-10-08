using Shears;
using System;
using UnityEngine;

namespace SoulTower.Traps
{
    public class TrapSlot : MonoBehaviour
    {
        [SerializeField, ReadOnly] private Trap trap;
        [SerializeField] private TrapPlacementType placementType;

        public Trap Trap => trap;
        public TrapPlacementType PlacementType => placementType;
        internal TrapSlotGroup Group { get; set; }

        public event Action<Trap> TrapUpdated;

        public void PlaceTrap(Trap trap)
        {
            this.trap = trap;

            TrapUpdated?.Invoke(trap);
        }

        public bool CanPlaceTrap(Trap trap)
        {
            if (trap.Size == 1 && this.trap == null && (placementType & trap.PlacementType) != 0)
                return true;
            else if (Group != null)
                return Group.CanPlaceTrap(trap, this);
            else
                return false;
        }
    }
}
