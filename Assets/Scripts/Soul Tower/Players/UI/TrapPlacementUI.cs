using Shears.Input;
using Shears.Logging;
using Shears.UI;
using SoulTower.Traps;
using SoulTower.Traps.UI;
using System.Collections;
using UnityEngine;

namespace SoulTower.Players.UI
{
    public class TrapPlacementUI : SHMonoBehaviourLogger
    {
        [Header("Components")]
        [SerializeField] private TrapSlotInteractor interactor;
        [SerializeField] private ManagedInputProvider inputProvider;

        [Header("Trap")]
        [SerializeField] private Trap trap;
        [SerializeField] private Material hologramMaterial;

        private IManagedInput cancelInput;
        private TrapModel hologram;
        private bool isPlacing = false;

        private void Awake()
        {
            cancelInput = inputProvider.GetInput("Cancel Placement");
        }

        private void OnEnable()
        {
            interactor.Interacted += OnInteracted;
        }

        private void OnDisable()
        {
            interactor.Interacted -= OnInteracted;
            cancelInput.Performed -= OnCancelInput;
        }

        private void OnInteracted()
        {
            EndPlacing();
        }

        private void OnCancelInput(ManagedInputInfo info)
        {
            EndPlacing();
        }

        public void BeginPlacing(Trap trap)
        {
            if (isPlacing)
                return;

            this.trap = trap;
            interactor.CurrentTrap = trap;
            interactor.Enable();
            cancelInput.Performed += OnCancelInput;

            CreateHologram();

            isPlacing = true;
        }

        private void EndPlacing()
        {
            if (!isPlacing)
                return;

            interactor.Disable();
            cancelInput.Performed -= OnCancelInput;

            ClearHologram();

            isPlacing = false;
        }

        private void CreateHologram()
        {
            if (hologram != null)
                return;

            var model = trap.GetComponentInChildren<TrapModel>();

            hologram = Instantiate(model);
            hologram.SetMaterial(hologramMaterial);

            StartCoroutine(IEMoveHologram());
        }

        private IEnumerator IEMoveHologram()
        {
            while (true)
            {
                if (interactor.HoveredTrapSlot != null)
                {
                    if (interactor.HoveredTrapSlot.CanPlaceTrap(trap))
                    {
                        hologramMaterial.color = Color.green;
                        SnapHologramPosition();
                    }
                    else
                    {
                        hologramMaterial.color = Color.red;
                        MoveHologram();
                    }
                }
                else
                {
                    hologramMaterial.color = Color.red;
                    MoveHologram();
                }

                yield return null;
            }
        }

        private void SnapHologramPosition()
        {
            var slot = interactor.HoveredTrapSlot;

            hologram.transform.SetPositionAndRotation(slot.GetTrapPosition(trap), slot.GetTrapRotation());
        }

        private void MoveHologram()
        {
            var cam = Camera.main;

            Ray ray = cam.ScreenPointToRay(ManagedPointer.Current.Position);
            Plane plane = new(Vector3.back, 0f);

            plane.Raycast(ray, out float enter);

            hologram.transform.position = ray.GetPoint(enter);
        }

        private void ClearHologram()
        {
            StopAllCoroutines();

            Destroy(hologram.gameObject);
        }
    }
}
