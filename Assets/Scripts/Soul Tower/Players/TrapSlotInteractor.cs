using Shears.Interaction;
using Shears.Logging;
using SoulTower.Traps;
using UnityEngine;

namespace SoulTower.Players
{
    public class TrapSlotInteractor : Interactor<TrapSlotInteractable>, ISHLoggable
    {
        [SerializeField] private Trap currentTrap;
        [field: SerializeField] public SHLogLevels LogLevels { get; set; } = SHLogLevels.Log | SHLogLevels.Issues;


        public override void TypeInteract(TrapSlotInteractable interactable)
        {
            if (currentTrap == null)
            {
                this.Log("No trap selected!", SHLogLevels.Warning);
                return;
            }

            var trap = Instantiate(currentTrap);

            interactable.TrapSlot.PlaceTrap(trap);
        }
    }
}
