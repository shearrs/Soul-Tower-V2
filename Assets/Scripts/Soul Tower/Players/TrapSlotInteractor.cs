using Shears.Detection;
using Shears.Input;
using Shears.Interaction;
using Shears.Logging;
using SoulTower.Traps;
using System;
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
        private bool isEnabled = false;

        public Trap CurrentTrap { get => currentTrap; set => currentTrap = value; }

        public event Action Interacted;

        private void Awake()
        {
            interactInput = inputProvider.GetInput("Interact");
        }

        private void OnDisable()
        {
            interactInput.Performed -= OnInteractInput;
        }

        public void Enable()
        {
            if (isEnabled)
                return;

            interactInput.Performed += OnInteractInput;

            isEnabled = true;
        }

        public void Disable()
        {
            if (!isEnabled)
                return;

            interactInput.Performed -= OnInteractInput;

            isEnabled = false;
        }

        private void OnInteractInput(ManagedInputInfo info) => TryDetect();

        private void TryDetect()
        {
            detector.Detect();

            if (!detector.TryGetDetection(out TrapSlotInteractable interactable, true))
                return;

            interactable.Accept(this);
            Interacted?.Invoke();
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
