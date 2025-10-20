using Shears;
using Shears.Input;
using Shears.Signals;
using Shears.Logging;
using SoulTower.Traps;
using SoulTower.Traps.UI;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using SoulTower.Currency;

namespace SoulTower.Players.UI
{
    public class TrapPlacementUI : SHMonoBehaviourLogger
    {
        [Header("Components")]
        [SerializeField] private TrapSlotInteractor interactor;

        [Header("Trap")]
        [SerializeField] private Trap trap;
        [SerializeField] private Material hologramMaterial;

        private readonly List<TrapSummonButton> buttons = new();
        private TrapModel hologram;
        private float alpha;

        private void Awake()
        {
            GetComponentsInChildren(buttons);

            hologramMaterial = Instantiate(hologramMaterial);
            alpha = hologramMaterial.color.a;

            UpdateButtonClickability(SoulsManager.SoulPoints);
        }

        private void OnEnable()
        {
            SignalShuttle.Register<SoulsChangedSignal>(OnSoulsChanged);

            interactor.BeganPlacing += BeginPlacing;
            interactor.EndedPlacing += EndPlacing;

            foreach (var button in buttons)
                button.Clicked += OnTrapButtonClicked;
        }

        private void OnDisable()
        {
            SignalShuttle.Deregister<SoulsChangedSignal>(OnSoulsChanged);

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
            ClearHologram();

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

            if (hologram != null)
                Destroy(hologram.gameObject);
        }

        private void OnSoulsChanged(SoulsChangedSignal signal)
        {
            UpdateButtonClickability(signal.Souls);
        }

        private void UpdateButtonClickability(int souls)
        {
            foreach (var button in buttons)
                button.SetSelectable(button.Trap.Cost < souls);
        }
    }
}
