using Shears;
using Shears.Logging;
using SoulTower.Towers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SoulTower.Traps
{
    public class TrapSlotGroup : TileSubgroup
    {
#pragma warning disable CS0414
        [SerializeField] private TrapSlot slot;
#pragma warning restore CS0414

        [SerializeField] private List<TrapSlotSubgroup> subgroups = new();

        private readonly List<TrapSlot> slotInstances = new();
        private readonly List<TrapSlot> currentSelection = new();
        private readonly List<TrapSlot> leftMultiGroupDiv = new();
        private readonly List<TrapSlot> rightMultiGroupDiv = new();

        public event Action<TrapSlotSubgroup> TrapPlaced;
        public event Action<TrapSlotSubgroup> TrapRemoved;
        public event Action<TrapSlotSubgroup> MultigroupUpdated;

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
            if (trap.TryGetComponent<ShockSurface>(out _))
            {
                TryMultiTrapPlacement(trap, selectedSlot);
                return;
            }

            if (!FindValidGroup(trap, selectedSlot))
            {
                SHLogger.Log($"Could not find valid group for trap {trap.name}!", SHLogLevels.Error, context: selectedSlot);
                return;
            }

            StandardTrapPlacement(trap, selectedSlot);
        }

        public void RemoveTrap(Trap trap, TrapSlot selectedSlot)
        {
            int groupIndex = GetSubgroupIndex(selectedSlot);

            if (groupIndex == -1)
            {
                SHLogger.Log($"Could not find slot {selectedSlot.name} in subgroups!", SHLogLevels.Error);
                return;
            }

            var group = subgroups[groupIndex];

            if (group.IsMultigroup)
            {
                RemoveTrapFromMultigroup(trap, selectedSlot, groupIndex);
                return;
            }

            foreach (var slot in group.Slots)
                slot.SetTrap(null);

            subgroups.RemoveAt(groupIndex);
            Destroy(trap.gameObject);

            TrapRemoved?.Invoke(group);
        }

        private void RemoveTrapFromMultigroup(Trap trap, TrapSlot selectedSlot, int groupIndex)
        {
            leftMultiGroupDiv.Clear();
            rightMultiGroupDiv.Clear();

            bool foundSelected = false;
            bool onCooldown = false;
            var group = subgroups[groupIndex];

            foreach (var slot in group.Slots)
            {
                if (slot.Trap.IsOnCooldown)
                    onCooldown = true;

                if (slot == selectedSlot)
                {
                    foundSelected = true;
                    continue;
                }

                if (!foundSelected)
                    leftMultiGroupDiv.Add(slot);
                else
                    rightMultiGroupDiv.Add(slot);
            }

            subgroups.RemoveAt(groupIndex);
            Destroy(trap.gameObject);
            TrapRemoved?.Invoke(group);

            if (leftMultiGroupDiv.Count > 0)
            {
                var leftGroup = GetDividedMultigroup(leftMultiGroupDiv);

                if (onCooldown)
                {
                    if (leftGroup.IsMultigroup)
                    {
                        foreach (var multiTrap in leftGroup.Traps)
                            multiTrap.BeginCooldown();
                    }
                    else
                        leftGroup.Trap.BeginCooldown();
                }

                subgroups.Add(leftGroup);
                MultigroupUpdated?.Invoke(leftGroup);
            }

            if (rightMultiGroupDiv.Count > 0)
            {
                var rightGroup = GetDividedMultigroup(rightMultiGroupDiv);

                if (onCooldown)
                {
                    if (rightGroup.IsMultigroup)
                    {
                        foreach (var multiTrap in rightGroup.Traps)
                            multiTrap.BeginCooldown();
                    }
                    else
                        rightGroup.Trap.BeginCooldown();
                }

                subgroups.Add(rightGroup);
                MultigroupUpdated?.Invoke(rightGroup);
            }
        }

        private TrapSlotSubgroup GetDividedMultigroup(List<TrapSlot> dividerList)
        {
            currentSelection.Clear();
            var trapList = new List<Trap>();

            foreach (var slot in dividerList)
            {
                currentSelection.Add(slot);
                trapList.Add(slot.Trap);
            }

            TrapSlotSubgroup group;

            if (dividerList.Count == 1)
                group = new(new(currentSelection), trapList[0]);
            else
                group = new(new(currentSelection), trapList);

            return group;
        }

        public bool CanPlaceTrap(Trap trap, TrapSlot selectedSlot)
        {
            if (trap == null)
            {
                SHLogger.Log("Trap is null!", SHLogLevels.Error);
                return false;
            }

            return FindValidGroup(trap, selectedSlot);
        }

        public Vector3 GetTrapPosition(Trap trap, TrapSlot selectedSlot)
        {
            FindValidGroup(trap, selectedSlot);

            return GetTrapPositionForCurrentGroup();
        }

        private Vector3 GetTrapPositionForCurrentGroup()
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

        private void StandardTrapPlacement(Trap trap, TrapSlot selectedSlot)
        {
            var slots = new List<TrapSlot>();
            slots.AddRange(currentSelection);
            var group = new TrapSlotSubgroup(slots, trap);

            foreach (var slot in currentSelection)
                slot.SetTrap(trap);

            subgroups.Add(group);

            trap.transform.SetParent(selectedSlot.TrapContainer);
            trap.transform.SetPositionAndRotation(GetTrapPositionForCurrentGroup(), selectedSlot.GetTrapRotation());
            TrapPlaced?.Invoke(group);
        }

        private void TryMultiTrapPlacement(Trap trap, TrapSlot selectedSlot)
        {
            if (!CanPlaceTrapIgnoreSize(trap, selectedSlot))
            {
                SHLogger.Log("Can't place shock trap, not enough room.", SHLogLevels.Verbose);
                return;
            }

            int slotIndex = slotInstances.IndexOf(selectedSlot);

            if (slotIndex == -1)
            {
                SHLogger.Log("Could not find slot in group: " + selectedSlot, SHLogLevels.Error);
                return;
            }

            TrapSlot leftNeighbor = null;
            TrapSlot rightNeighbor = null;

            if (slotIndex > 0)
            {
                var neighbor = slotInstances[slotIndex - 1];

                if (neighbor.Trap != null && neighbor.Trap.TryGetComponent(out ShockSurface _))
                    leftNeighbor = neighbor;
            }

            if (slotIndex < slotInstances.Count - 1)
            {
                var neighbor = slotInstances[slotIndex + 1];

                if (neighbor.Trap != null && neighbor.Trap.TryGetComponent(out ShockSurface _))
                    rightNeighbor = neighbor;
            }

            if (leftNeighbor != null || rightNeighbor != null)
            {
                List<Trap> traps = new();
                currentSelection.Clear();

                if (leftNeighbor != null)
                {
                    int leftGroupIndex = GetSubgroupIndex(leftNeighbor);
                    foreach (var slot in subgroups[leftGroupIndex].Slots)
                    {
                        currentSelection.Add(slot);
                        traps.Add(slot.Trap);
                    }

                    subgroups.RemoveAt(leftGroupIndex);
                }

                currentSelection.Add(selectedSlot); // this keeps the slot order: left, selected, right

                if (rightNeighbor != null)
                {
                    int rightGroupIndex = GetSubgroupIndex(rightNeighbor);

                    foreach (var slot in subgroups[rightGroupIndex].Slots)
                    {
                        currentSelection.Add(slot);
                        traps.Add(slot.Trap);
                    }

                    subgroups.RemoveAt(rightGroupIndex);
                }

                var slots = new List<TrapSlot>(currentSelection);
                traps.Add(trap);
                selectedSlot.SetTrap(trap);
                var multigroup = new TrapSlotSubgroup(slots, traps);

                foreach (var multiTrap in traps)
                    multiTrap.ResetCooldown();

                subgroups.Add(multigroup);

                trap.transform.SetParent(selectedSlot.TrapContainer);
                trap.transform.SetPositionAndRotation(selectedSlot.TrapContainer.position, selectedSlot.GetTrapRotation());
                TrapPlaced?.Invoke(multigroup);
            }
            else
                StandardTrapPlacement(trap, selectedSlot);
        }

        private int GetSubgroupIndex(TrapSlot slot)
        {
            for (int i = 0; i < subgroups.Count; i++)
            {
                if (subgroups[i].Slots.Contains(slot))
                    return i;
            }

            return -1;
        }
    }
}
