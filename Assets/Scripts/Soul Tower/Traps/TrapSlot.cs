using Shears;
using Shears.Logging;
using System;
using UnityEngine;

namespace SoulTower.Traps
{
    public class TrapSlot : SHMonoBehaviourLogger
    {
        [Header("Trap Slot")]
        [SerializeField, ReadOnly] private Trap trap;
        [SerializeField, ReadOnly] private TrapPlacementType placementType;
        [SerializeField] private Transform trapContainer;

        private TrapSlotGroup group;

        internal Transform TrapContainer => trapContainer;
        internal TrapSlotGroup Group { get => group; set => group = value; }
        public Trap Trap => trap;
        public TrapPlacementType PlacementType { get => placementType; internal set => placementType = value; }

        public void PlaceTrap(Trap trap) => group.PlaceTrap(trap, this);
        public bool CanPlaceTrap(Trap trap) => group.CanPlaceTrap(trap, this);

        public Vector3 GetTrapPosition(Trap trap) => group.GetTrapPosition(trap, this);
        public Quaternion GetTrapRotation() => trapContainer.rotation;

        internal void SetTrap(Trap trap)
        {
            this.trap = trap;
        }
    }
}
