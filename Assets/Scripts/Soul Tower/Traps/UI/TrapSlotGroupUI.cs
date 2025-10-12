using Shears.Logging;
using Shears.UI;
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
        }

        private void OnDisable()
        {
            group.TrapPlaced -= OnTrapPlaced;
        }

        private void OnTrapPlaced(TrapSlotSubgroup subgroup)
        {
            if (trapButtons.ContainsKey(subgroup.Trap))
            {
                SHLogger.Log("UI already contains trap!", SHLogLevels.Warning);
                return;
            }

            Vector3 position = Vector3.zero;
            var (slots, trap) = (subgroup.Slots, subgroup.Trap);

            foreach (var slot in slots)
                position += slot.transform.position;

            position /= slots.Count;

            var button = Instantiate(buttonPrefab, transform);
            button.transform.position = position;

            button.Trap = trap;
            button.Enable();

            trapButtons[trap] = button;

            trap.Activated += OnTrapActivated;
            trap.CooldownCompleted += OnTrapCooldownCompleted;
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
