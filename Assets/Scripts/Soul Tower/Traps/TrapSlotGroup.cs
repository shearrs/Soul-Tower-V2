using Shears.Logging;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Traps
{
    public class TrapSlotGroup : SHMonoBehaviourLogger
    {
        [SerializeField] private List<TrapSlot> slots;
        private readonly List<TrapSlot> currentSelection = new();

        private void Awake()
        {
            foreach (var slot in slots)
                slot.Group = this;
        }

        public bool CanPlaceTrap(Trap trap, TrapSlot selectedSlot)
        {
            return FindValidGroup(trap, selectedSlot);
        }

        public void PlaceTrap(Trap trap, TrapSlot selectedSlot)
        {
            FindValidGroup(trap, selectedSlot);

            foreach (var slot in currentSelection)
                slot.Trap = trap;

            Vector3 pos = GetTrapPosition(trap);
            trap.transform.SetParent(selectedSlot.TrapContainer);
            trap.transform.SetPositionAndRotation(pos, selectedSlot.GetTrapRotation());
        }

        public Vector3 GetTrapPosition(Trap trap, TrapSlot selectedSlot)
        {
            FindValidGroup(trap, selectedSlot);

            return GetTrapPosition(trap);
        }

        private Vector3 GetTrapPosition(Trap trap)
        {
            Vector3 pos = Vector3.zero;

            foreach (var slot in currentSelection)
                pos += slot.GetDefaultTrapPosition();

            pos /= currentSelection.Count;

            return pos;
        }

        private bool FindValidGroup(Trap trap, TrapSlot startingSlot)
        {
            currentSelection.Clear();

            if (CanPlaceTrapIgnoreSize(trap, startingSlot))
                currentSelection.Add(startingSlot);
            else
                return false;

            int selectedIndex = slots.IndexOf(startingSlot);
            bool hasLeftNeighbor = selectedIndex != -1 && selectedIndex - 1 != -1;
            bool hasRightNeighbor = selectedIndex != -1 && selectedIndex + 1 < slots.Count;
            bool validLeftNeighbor = hasLeftNeighbor && CanPlaceTrapIgnoreSize(trap, slots[selectedIndex - 1]);
            bool validRightNeighbor = hasRightNeighbor && CanPlaceTrapIgnoreSize(trap, slots[selectedIndex + 1]);

            if (trap.Size == 1)
                return true;
            else if (trap.Size == 2)
            {
                if (validLeftNeighbor)
                {
                    currentSelection.Add(slots[selectedIndex - 1]);
                    return true;
                }
                else if (validRightNeighbor)
                {
                    currentSelection.Add(slots[selectedIndex + 1]);
                    return true;
                }
            }
            else if (trap.Size == 3 && validLeftNeighbor && validRightNeighbor)
            {
                currentSelection.Add(slots[selectedIndex - 1]);
                currentSelection.Add(slots[selectedIndex + 1]);

                return true;
            }

            return false;
        }

        private bool CanPlaceTrapIgnoreSize(Trap trap, TrapSlot selectedSlot)
        {
            return selectedSlot.Trap == null && (selectedSlot.PlacementType & trap.PlacementType) != 0;
        }
    }
}
