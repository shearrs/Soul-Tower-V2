using Shears;
using Shears.Tweens;
using Shears.UI;
using System;
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

        private Trap trap;
        private Tween moveTween;
        private Tween scaleTween;

        public Trap Trap { get => trap; set => trap = value; }

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

        private void OnButtonClicked()
        {
            Trap.Activate();
        }

        public void Use()
        {
            button.Selectable = false;

            Vector3 position = button.transform.localPosition;
            position.z = buttonMovementRange.Max;

            moveTween.Dispose();
            scaleTween.Dispose();
            moveTween = button.transform.DoMoveLocalTween(position, pressTweenData);
            scaleTween = button.transform.DoScaleLocalTween(smallButtonScale, pressTweenData);
        }

        public void ResetForUse()
        {
            Vector3 position = button.transform.localPosition;
            position.z = buttonMovementRange.Min;

            moveTween.Dispose();
            scaleTween.Dispose();
            moveTween = button.transform.DoMoveLocalTween(position, pressTweenData);
            scaleTween = button.transform.DoScaleLocalTween(defaultButtonScale, pressTweenData);

            moveTween.Completed += () => button.Selectable = true;
        }
    }
}
