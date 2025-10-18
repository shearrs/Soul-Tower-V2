using Shears;
using Shears.Detection;
using Shears.Input;
using Shears.Logging;
using SoulTower.Traps;
using System;
using System.Collections;
using UnityEngine;

namespace SoulTower.Players
{
    public class TrapSlotInteractor : SHMonoBehaviourLogger
    {
        [Header("Interactor")]
        [SerializeField, ReadOnly] private Trap currentTrap;
        [SerializeField] private AreaDetector3D detector;
        [SerializeField] private ManagedInputProvider inputProvider;

        private IManagedInput interactInput;
        private IManagedInput altInteractInput;
        private TrapSlot hoveredTrapSlot;
        private bool isEnabled = false;

        public TrapSlot HoveredTrapSlot => hoveredTrapSlot;

        public event Action Interacted;

        private void Awake()
        {
            interactInput = inputProvider.GetInput("Interact");
            altInteractInput = inputProvider.GetInput("Alternative Interact");

            Enable();
        }

        private void OnDisable()
        {
            interactInput.Performed -= OnInteractInput;
            altInteractInput.Performed -= OnAltInteractInput;
        }

        public void Enable()
        {
            if (isEnabled)
                return;

            StartCoroutine(IEUpdateHover());
            interactInput.Performed += OnInteractInput;
            altInteractInput.Performed += OnAltInteractInput;

            isEnabled = true;
        }

        public void Disable()
        {
            if (!isEnabled)
                return;

            StopAllCoroutines();
            interactInput.Performed -= OnInteractInput;
            altInteractInput.Performed -= OnAltInteractInput;

            isEnabled = false;
        }

        public void BeginPlacing(Trap trap)
        {
            currentTrap = trap;
            altInteractInput.Disable();
        }

        public void EndPlacing()
        {
            currentTrap = null;
            altInteractInput.Enable();
        }

        private IEnumerator IEUpdateHover()
        {
            while (true)
            {
                TryHover();

                yield return null;
            }
        }

        private void OnInteractInput(ManagedInputInfo info) => TryInteract();

        private void OnAltInteractInput(ManagedInputInfo info) => TryAltInteract();

        private void TryHover()
        {
            detector.Detect();
            detector.TryGetDetection(out TrapSlot slot, true);

            if (slot == hoveredTrapSlot)
                return;

            hoveredTrapSlot = slot;
        }

        private void TryInteract()
        {
            if (hoveredTrapSlot != null)
                Interact(hoveredTrapSlot);
        }

        private void Interact(TrapSlot slot)
        {
            if (currentTrap == null)
            {
                Log("No trap selected.", SHLogLevels.Verbose);
                return;
            }

            if (!slot.CanPlaceTrap(currentTrap))
            {
                Log($"Can not place {currentTrap.name} ({currentTrap.PlacementType}) on slot with type: {slot.PlacementType}", SHLogLevels.Verbose);
                return;
            }

            var trap = Instantiate(currentTrap);
            slot.PlaceTrap(trap);

            Interacted?.Invoke();
        }

        private void TryAltInteract()
        {
            if (hoveredTrapSlot != null)
                AltInteract(hoveredTrapSlot);
        }

        private void AltInteract(TrapSlot slot)
        {
            if (slot.Trap == null)
            {
                Log("Slot has no trap.", SHLogLevels.Verbose);
                return;
            }

            slot.RemoveTrap();
        }
    }
}
