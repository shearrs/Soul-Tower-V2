using Shears;
using Shears.Tweens;
using Shears.UI;
using System;
using UnityEngine;

namespace SoulTower.Traps.UI
{
    [RequireComponent(typeof(MeshButton))]
    public class ActivateTrapButton : MonoBehaviour
    {
        [Header("Tween Settings")]
        [SerializeField] private Range<float> buttonMovementRange;
        [SerializeField] private Vector3 defaultButtonScale = Vector3.one;
        [SerializeField] private Vector3 smallButtonScale = Vector3.one * 0.5f;
        [SerializeField] private TweenData pressTweenData;

        private Trap trap;
        private MeshButton button;
        private Tween moveTween;
        private Tween scaleTween;

        private MeshButton Button
        {
            get
            {
                if (button == null)
                    button = GetComponent<MeshButton>();

                return button;
            }
        }
        public Trap Trap { get => trap; set => trap = value; }

        private void Awake()
        {
            button = GetComponent<MeshButton>();
        }

        private void OnEnable()
        {
            Button.Clicked += OnButtonClicked;
        }

        private void OnDisable()
        {
            Button.Clicked -= OnButtonClicked;
        }

        public void Enable()
        {
            Button.Enable();
            Button.Selectable = true;
        }

        public void Disable()
        {
            Button.Disable();
            Button.transform.localScale = defaultButtonScale;
        }

        private void OnButtonClicked()
        {
            Trap.Activate();
        }

        public void Use()
        {
            Button.Selectable = false;

            Vector3 position = Button.transform.localPosition;
            position.z = buttonMovementRange.Max;

            moveTween.Dispose();
            scaleTween.Dispose();
            moveTween = Button.transform.DoMoveLocalTween(position, pressTweenData);
            scaleTween = Button.transform.DoScaleLocalTween(smallButtonScale, pressTweenData);
        }

        public void ResetForUse()
        {
            Vector3 position = Button.transform.localPosition;
            position.z = buttonMovementRange.Min;

            moveTween.Dispose();
            scaleTween.Dispose();
            moveTween = Button.transform.DoMoveLocalTween(position, pressTweenData);
            scaleTween = Button.transform.DoScaleLocalTween(defaultButtonScale, pressTweenData);

            moveTween.Completed += () => Button.Selectable = true;
        }
    }
}
