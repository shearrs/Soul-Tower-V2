using Shears.Interaction;
using SoulTower.Traps;
using UnityEngine;

namespace SoulTower.Players
{
    public class TrapSlotInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] private TrapSlot trapSlot;

        public TrapSlot TrapSlot => trapSlot;

        public void Accept(IInteractor interactor)
        {
            interactor.Interact(this);
        }
    }
}
