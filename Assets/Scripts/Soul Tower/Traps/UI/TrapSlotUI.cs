using UnityEngine;

namespace SoulTower.Traps.UI
{
    public class TrapSlotUI : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private TrapSlot slot;
        [SerializeField] private ActivateTrapButton button;

        private Trap currentTrap;

        public Vector3 ButtonPosition => button.transform.position;

        private void OnEnable()
        {
            slot.TrapChanged += OnTrapChanged;
        }

        private void OnDisable()
        {
            slot.TrapChanged -= OnTrapChanged;
        }

        private void OnTrapChanged()
        {
            if (currentTrap == slot.Trap)
                return;

            if (currentTrap != null)
            {
                currentTrap.Activated -= OnTrapActivated;
                currentTrap.CooldownCompleted -= OnTrapCooldownCompleted;
            }

            currentTrap = slot.Trap;
            button.Trap = currentTrap;

            if (currentTrap != null && !slot.UsedForGroup)
            {
                currentTrap.Activated += OnTrapActivated;
                currentTrap.CooldownCompleted += OnTrapCooldownCompleted;

                button.Enable();
            }
            else
                button.Disable();
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
