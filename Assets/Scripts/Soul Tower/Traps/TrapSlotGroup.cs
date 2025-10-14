using Shears;
using Shears.Logging;
using SoulTower.Towers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Traps
{
    [SelectionBase]
    public class TrapSlotGroup : TileSubgroup
    {
#pragma warning disable CS0414
        [SerializeField] private TrapSlot slot;
#pragma warning restore CS0414

        [SerializeField, ReadOnly] private List<TrapSlotSubgroup> subgroups = new();

        private readonly List<TrapSlot> slotInstances = new();
        private readonly List<TrapSlot> currentSelection = new();

        public event Action<TrapSlotSubgroup> TrapPlaced;

        private void OnValidate()
        {
            GetComponentsInChildren(slotInstances);
        }

        private void Awake()
        {
            foreach (var slot in slotInstances)
                slot.Group = this;
        }

        public void PlaceTrap(Trap trap, TrapSlot selectedSlot)
        {
            if (!FindValidGroup(trap, selectedSlot))
            {
                SHLogger.Log($"Could not find valid group for trap {trap.name}!", SHLogLevels.Error, context: selectedSlot);
                return;
            }

            var list = new List<TrapSlot>();
            list.AddRange(currentSelection);
            var group = new TrapSlotSubgroup(list, trap);

            foreach (var slot in currentSelection)
                slot.SetTrap(trap);

            subgroups.Add(group);

            trap.transform.SetParent(selectedSlot.TrapContainer);
            trap.transform.SetPositionAndRotation(GetTrapPositionForCurrentGroup(trap), selectedSlot.GetTrapRotation());
            TrapPlaced?.Invoke(group);
        }

        public bool CanPlaceTrap(Trap trap, TrapSlot selectedSlot)
        {
            return FindValidGroup(trap, selectedSlot);
        }

        public Vector3 GetTrapPosition(Trap trap, TrapSlot selectedSlot)
        {
            FindValidGroup(trap, selectedSlot);

            return GetTrapPositionForCurrentGroup(trap);
        }

        private Vector3 GetTrapPositionForCurrentGroup(Trap trap)
        {
            Vector3 position = Vector3.zero;

            foreach (var slot in currentSelection)
                position += slot.TrapContainer.position;

            return position / currentSelection.Count;
        }

        private bool FindValidGroup(Trap trap, TrapSlot selectedSlot)
        {
            currentSelection.Clear();

            if (CanPlaceTrapIgnoreSize(trap, selectedSlot))
                currentSelection.Add(selectedSlot);
            else
                return false;

            if (trap.Size == 1)
                return true;

            int selectedIndex = slotInstances.IndexOf(selectedSlot);
            int leftIndex = selectedIndex;
            int rightIndex = selectedIndex;
            bool couldntFindSlot = false;

            for (int i = 2; i <= trap.Size; i++)
            {
                if (i % 2 == 0)
                {
                    if (TryGetValidLeftSlot(trap, leftIndex, out var left))
                    {
                        leftIndex--;
                        currentSelection.Add(left);
                    }
                    else if (TryGetValidRightSlot(trap, rightIndex, out var right))
                    {
                        rightIndex++;
                        currentSelection.Add(right);
                    }
                    else
                    {
                        couldntFindSlot = true;
                        break;
                    }
                }
                else
                {
                    if (TryGetValidRightSlot(trap, rightIndex, out var right))
                    {
                        rightIndex++;
                        currentSelection.Add(right);
                    }
                    else if (TryGetValidLeftSlot(trap, leftIndex, out var left))
                    {
                        leftIndex--;
                        currentSelection.Add(left);
                    }
                    else
                    {
                        couldntFindSlot = true; 
                        break;
                    }
                }
            }

            return !couldntFindSlot;
        }

        private bool TryGetValidLeftSlot(Trap trap, int originIndex, out TrapSlot left)
        {
            left = null;

            if (originIndex <= 0)
                return false;

            var slot = slotInstances[originIndex - 1];

            if (CanPlaceTrapIgnoreSize(trap, slot))
            {
                left = slot;
                return true;
            }

            return false;
        }

        private bool TryGetValidRightSlot(Trap trap, int originIndex, out TrapSlot right)
        {
            right = null;

            if (originIndex == slotInstances.Count - 1)
                return false;

            var slot = slotInstances[originIndex + 1];

            if (CanPlaceTrapIgnoreSize(trap, slot))
            {
                right = slot;
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
