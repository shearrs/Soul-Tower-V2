using Shears.Logging;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Traps.UI
{
    public class TrapSlotGroupUI : MonoBehaviour
    {
        [SerializeField] private TrapSlotGroup group;
        [SerializeField] private ActivateTrapButton buttonPrefab;

        private readonly Dictionary<Trap, ActivateTrapButton> trapButtons = new();

        private void OnEnable()
        {
            group.TrapPlaced += OnTrapPlaced;
            group.TrapRemoved += OnTrapRemoved;
            group.MultigroupUpdated += OnMultigroupUpdated;
        }

        private void OnDisable()
        {
            group.TrapPlaced -= OnTrapPlaced;
            group.TrapRemoved -= OnTrapRemoved;
            group.MultigroupUpdated -= OnMultigroupUpdated;
        }

        private void OnTrapPlaced(TrapSlotSubgroup subgroup)
        {
            if (subgroup.IsMultigroup)
            {
                OnMultigroupPlaced(subgroup);
                return;
            }

            if (trapButtons.ContainsKey(subgroup.Trap))
            {
                SHLogger.Log("UI already contains trap!", SHLogLevels.Warning);
                return;
            }

            if (subgroup.Trap.IsPassive)
                return;

            Vector3 position = Vector3.zero;
            var (slots, trap) = (subgroup.Slots, subgroup.Trap);

            foreach (var slot in slots)
                position += slot.transform.position;

            position /= slots.Count;

            var button = Instantiate(buttonPrefab, transform);
            button.transform.position = position;

            button.AddTrap(trap);
            button.Enable();

            if (trap.IsOnCooldown)
                button.Use();

            trapButtons[trap] = button;

            trap.Activated += OnTrapActivated;
            trap.CooldownCompleted += OnTrapCooldownCompleted;
        }

        private void OnMultigroupPlaced(TrapSlotSubgroup multigroup)
        {
            foreach (var trap in multigroup.Traps)
            {
                if (trapButtons.TryGetValue(trap, out var existingButton))
                {
                    Destroy(existingButton.gameObject);
                    trapButtons.Remove(trap);
                }
            }

            Vector3 position = Vector3.zero;
            var (slots, traps) = (multigroup.Slots, multigroup.Traps);

            foreach (var slot in slots)
                position += slot.transform.position;

            position /= slots.Count;

            var button = Instantiate(buttonPrefab, transform);
            button.transform.position = position;

            bool isOnCooldown = false;

            foreach (var trap in traps)
            {
                if (trap.IsPassive)
                    continue;

                button.AddTrap(trap);
                trapButtons[trap] = button;

                if (trap.IsOnCooldown)
                    isOnCooldown = true;

                trap.Activated += OnTrapActivated;
                trap.CooldownCompleted += OnTrapCooldownCompleted;
            }

            button.Enable();

            if (isOnCooldown)
                button.Use();
        }

        private void OnTrapRemoved(TrapSlotSubgroup subgroup)
        {
            if (subgroup.IsMultigroup)
            {
                foreach (var trap in subgroup.Traps)
                {
                    if (!trapButtons.TryGetValue(trap, out var multiButton))
                    {
                        if (!trap.IsPassive)
                            SHLogger.Log("UI does not contain trap!", SHLogLevels.Warning);

                        continue;
                    }

                    trapButtons.Remove(trap);

                    if (multiButton != null)
                        Destroy(multiButton.gameObject);
                }

                return;
            }

            if (!trapButtons.TryGetValue(subgroup.Trap, out var button))
            {
                SHLogger.Log("UI does not contain trap!", SHLogLevels.Warning);
                return;
            }

            trapButtons.Remove(subgroup.Trap);

            if (button != null)
                Destroy(button.gameObject);
        }

        private void OnMultigroupUpdated(TrapSlotSubgroup group)
        {
            OnTrapPlaced(group);
        }

        private void OnTrapActivated(Trap trap)
        {
            if (trapButtons.TryGetValue(trap, out var button))
                button.Use();
            else
                SHLogger.Log($"Could not find button for trap {trap.name}!", SHLogLevels.Warning);
        }

        private void OnTrapCooldownCompleted(Trap trap)
        {
            if (trapButtons.TryGetValue(trap, out var button))
                button.ResetForUse();
            else
                SHLogger.Log($"Could not find button for trap {trap.name}!", SHLogLevels.Warning);
        }
    }
}
