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
        [SerializeField] private TrapPlacementType placementType;
        [SerializeField] private Transform trapContainer;

        private TrapSlotGroup group;

        internal Transform TrapContainer => trapContainer;
        internal TrapSlotGroup Group { get => group; set => group = value; }
        public Trap Trap { get => trap; internal set => trap = value; }
        public TrapPlacementType PlacementType => placementType;

        public event Action TrapChanged;

        public void PlaceTrap(Trap trap)
        {
            if (trap.Size == 1)
            {
                this.trap = trap;
                trap.transform.SetParent(trapContainer);
                trap.transform.SetLocalPositionAndRotation(GetDefaultTrapPosition(), GetTrapRotation());
            }
            else
                group.PlaceTrap(trap, this);

            TrapChanged?.Invoke();
        }

        public Vector3 GetDefaultTrapPosition() => trapContainer.position;

        public Vector3 GetTrapPosition(Trap trap)
        {
            if (trap.Size == 1)
                return trapContainer.position;
            else if (group != null)
                return group.GetTrapPosition(trap, this);
            else
                return Vector3.zero;
        }

        public Quaternion GetTrapRotation() => trapContainer.rotation;

        public bool CanPlaceTrap(Trap trap)
        {
            if (this.trap != null || (placementType & trap.PlacementType) == 0)
                return false;

            if (trap.Size == 1)
                return true;
            else
                return group != null && group.CanPlaceTrap(trap, this);
        }
    }
}
