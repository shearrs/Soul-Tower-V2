using Shears;
using Shears.Tweens;
using Shears.UI;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace SoulTower.Traps.UI
{
    public class ActivateTrapButton : MonoBehaviour
    {
        [SerializeField] private MeshButton button;

        [Header("Tween Settings")]
        [SerializeField] private Range<float> buttonMovementRange;
        [SerializeField] private Vector3 defaultButtonScale = Vector3.one;
        [SerializeField] private Vector3 smallButtonScale = Vector3.one * 0.5f;
        [SerializeField] private TweenData pressTweenData;

        private readonly HashSet<Trap> traps = new();
        private Tween moveTween;
        private Tween scaleTween;
        private bool isUsing = false;
        private bool isResetting = false;

        public IReadOnlyCollection<Trap> Traps => traps;

        private void OnEnable()
        {
            button.Clicked += OnButtonClicked;
        }

        private void OnDisable()
        {
            button.Clicked -= OnButtonClicked;
        }

        public void Enable()
        {
            button.Enable();
            button.Selectable = true;
        }

        public void Disable()
        {
            button.Disable();
            button.transform.localScale = defaultButtonScale;
        }

        public void AddTrap(Trap trap)
        {
            if (traps.Contains(trap))
                return;

            traps.Add(trap);
        }

        public void RemoveTrap(Trap trap)
        {
            traps.Remove(trap);
        }

        public void Use()
        {
            if (isUsing || !button.Selectable)
                return;

            button.Selectable = false;
            isUsing = true;

            Vector3 position = button.transform.localPosition;
            position.z = buttonMovementRange.Max;

            moveTween.Dispose();
            scaleTween.Dispose();
            moveTween = button.transform.DoMoveLocalTween(position, pressTweenData);
            scaleTween = button.transform.DoScaleLocalTween(smallButtonScale, pressTweenData);

            moveTween.Completed += () => isUsing = false;
        }

        public void ResetForUse()
        {
            if (isResetting)
                return;

            isResetting = true;

            Vector3 position = button.transform.localPosition;
            position.z = buttonMovementRange.Min;

            moveTween.Dispose();
            scaleTween.Dispose();
            moveTween = button.transform.DoMoveLocalTween(position, pressTweenData);
            scaleTween = button.transform.DoScaleLocalTween(defaultButtonScale, pressTweenData);

            moveTween.Completed += () =>
            {
                button.Selectable = true;
                isResetting = false;
            };
        }

        private void OnButtonClicked()
        {
            foreach (var trap in traps)
                trap.Activate();
        }
    }
}
