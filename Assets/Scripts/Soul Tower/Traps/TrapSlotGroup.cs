using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Traps
{
    public class TrapSlotGroup : MonoBehaviour
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
            return TryGetValidGroup(trap, selectedSlot);
        }

        private bool TryGetValidGroup(Trap trap, TrapSlot startingSlot)
        {
            currentSelection.Clear();

            if (CanPlaceTrapIgnoreSize(trap, startingSlot))
                currentSelection.Add(startingSlot);
            else
                return false;

            int selectedIndex = slots.IndexOf(startingSlot);
            bool validLeftNeighbor = selectedIndex != -1 && selectedIndex - 1 != -1;
            bool validRightNeighbor = selectedIndex != -1 && selectedIndex + 1 < slots.Count;

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
