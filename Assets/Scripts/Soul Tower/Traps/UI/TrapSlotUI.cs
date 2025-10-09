using UnityEngine;

namespace SoulTower.Traps.UI
{
    public class TrapSlotUI : MonoBehaviour
    {
        [SerializeField] private TrapSlot slot;

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

        }
    }
}
