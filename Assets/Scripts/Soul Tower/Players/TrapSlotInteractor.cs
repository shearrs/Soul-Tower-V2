using Shears.Detection;
using Shears.Input;
using Shears.Interaction;
using Shears.Logging;
using SoulTower.Traps;
using UnityEngine;

namespace SoulTower.Players
{
    public class TrapSlotInteractor : Interactor<TrapSlotInteractable>, ISHLoggable
    {
        [field: SerializeField] public SHLogLevels LogLevels { get; set; } = SHLogLevels.Log | SHLogLevels.Issues;
        [SerializeField] private Trap currentTrap;
        [SerializeField] private AreaDetector3D detector;
        [SerializeField] private ManagedInputProvider inputProvider;

        private IManagedInput interactInput;

        // ManagedInputProvider.GetInput("Interact").Performed += TryDetect;

        private void Awake()
        {
            interactInput = inputProvider.GetInput("Interact");
        }

        private void OnEnable()
        {
            interactInput.Performed += OnInteractInput;
        }

        private void OnDisable()
        {
            interactInput.Performed -= OnInteractInput;
        }

        private void OnInteractInput(ManagedInputInfo info) => TryDetect();

        private void TryDetect()
        {
            detector.Detect();

            if (!detector.TryGetDetection(out TrapSlotInteractable interactable, true))
                return;

            interactable.Accept(this);
        }

        public override void TypeInteract(TrapSlotInteractable interactable)
        {
            if (currentTrap == null)
            {
                this.Log("No trap selected!", SHLogLevels.Warning);
                return;
            }

            if (!interactable.TrapSlot.CanPlaceTrap(currentTrap))
            {
                this.Log($"Can not place {currentTrap.name} ({currentTrap.PlacementType}) on slot with type: {interactable.TrapSlot.PlacementType}", SHLogLevels.Verbose);
                return;
            }

            var trap = Instantiate(currentTrap);

            interactable.TrapSlot.PlaceTrap(trap);
        }
    }
}
