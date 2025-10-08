using UnityEngine;

namespace SoulTower.Traps.UI
{
    public class TrapSlotUI : MonoBehaviour
    {
        [SerializeField] private TrapSlot trapSlot;
        [SerializeField] private Transform trapContainer;

        private void OnEnable()
        {
            trapSlot.TrapUpdated += OnTrapUpdated;
        }

        private void OnDisable()
        {
            trapSlot.TrapUpdated -= OnTrapUpdated;
        }

        private void OnTrapUpdated(Trap trap)
        {
            trap.transform.SetParent(trapContainer);
            trap.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }
    }
}
