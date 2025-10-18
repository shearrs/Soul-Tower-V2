using Shears;
using Shears.Input;
using Shears.Logging;
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
        [SerializeField] private TrapPlacementButton[] buttons;

        [Header("Trap")]
        [SerializeField] private Trap trap;
        [SerializeField] private Material hologramMaterial;

        private TrapModel hologram;
        private float alpha;

        private void Awake()
        {
            hologramMaterial = Instantiate(hologramMaterial);
            alpha = hologramMaterial.color.a;
        }

        private void OnEnable()
        {
            interactor.BeganPlacing += BeginPlacing;
            interactor.EndedPlacing += EndPlacing;

            foreach (var button in buttons)
                button.Clicked += OnTrapButtonClicked;
        }

        private void OnDisable()
        {
            interactor.BeganPlacing -= BeginPlacing;
            interactor.EndedPlacing -= EndPlacing;

            foreach (var button in buttons)
                button.Clicked -= OnTrapButtonClicked;
        }

        private void OnTrapButtonClicked(Trap trap)
        {
            interactor.BeginPlacing(trap);
        }

        private void BeginPlacing(Trap trap)
        {
            this.trap = trap;
            CreateHologram();
        }

        private void EndPlacing()
        {
            ClearHologram();
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
                        hologramMaterial.color = Color.green.With(a: alpha);
                        SnapHologramPosition();
                    }
                    else
                    {
                        hologramMaterial.color = Color.red.With(a: alpha);
                        MoveHologram();
                    }
                }
                else
                {
                    hologramMaterial.color = Color.red.With(a: alpha);
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
