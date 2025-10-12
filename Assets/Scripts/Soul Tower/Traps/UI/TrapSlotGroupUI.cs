using Shears.UI;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Traps.UI
{
    public class TrapSlotGroupUI : MonoBehaviour
    {
        [SerializeField] private TrapSlotGroup group;
        [SerializeField] private ActivateTrapButton button;

        private Trap currentTrap;

        private void OnEnable()
        {
            group.TrapPlaced += OnTrapPlaced;   
        }

        private void OnDisable()
        {
            group.TrapPlaced -= OnTrapPlaced;
        }

        private void OnTrapPlaced(Trap trap, IReadOnlyList<TrapSlot> slots)
        {
            Vector3 position = Vector3.zero;

            foreach (var slot in slots)
            {
                if (slot.TryGetComponent(out TrapSlotUI slotUI))
                    position += slotUI.ButtonPosition;
            }

            position /= slots.Count;

            if (currentTrap != null)
            {
                currentTrap.Activated -= OnTrapActivated;
                currentTrap.CooldownCompleted -= OnTrapCooldownCompleted;
            }

            currentTrap = trap;
            button.Trap = trap;

            if (currentTrap != null)
            {
                currentTrap.Activated += OnTrapActivated;
                currentTrap.CooldownCompleted += OnTrapCooldownCompleted;

                button.Enable();
            }
            else
                button.Disable();

            button.transform.position = position;
        }

        private void OnTrapActivated()
        {
            button.Use();
        }

        private void OnTrapCooldownCompleted()
        {
            button.ResetForUse();
        }
    }
}
