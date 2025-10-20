using Shears;
using Shears.Tweens;
using Shears.UI;
using UnityEngine;

namespace SoulTower.Players.UI
{
    public class StoreUI : MonoBehaviour
    {
        [SerializeField] private CanvasButton swapButton;
        [SerializeField] private RectTransform container;
        [SerializeField] private RectTransform trapUI;
        [SerializeField] private RectTransform roomUI;

        private readonly StructTweenData swapData = new(0.25f, easingFunction: TweenEase.EaseInOutQuad);

        private Tween exitTween;
        private Tween entryTween;
        private RectTransform currentUI;
        private RectTransform previousUI;

        private void Awake()
        {
            currentUI = trapUI;
        }

        private void OnEnable()
        {
            swapButton.Clicked += OnButtonClicked;
        }

        private void OnDisable()
        {
            swapButton.Clicked -= OnButtonClicked;

            exitTween.Dispose();
            entryTween.Dispose();

            exitTween.Completed -= OnExitComplete;
            entryTween.Completed -= OnEntryComplete;
        }

        private void OnButtonClicked()
        {
            if (exitTween.IsPlaying)
                return;

            previousUI = currentUI;
            currentUI = currentUI == trapUI ? roomUI : trapUI;
            currentUI.gameObject.SetActive(true);
            currentUI.anchoredPosition = Vector3.zero.With(x: container.rect.width);

            exitTween = previousUI.DoMoveLocalTween(Vector3.zero.With(x: container.rect.width), swapData);
            entryTween = currentUI.DoMoveLocalTween(Vector3.zero, swapData);

            exitTween.Completed += OnExitComplete;
            entryTween.Completed += OnEntryComplete;

            swapButton.Selectable = false;
        }

        private void OnExitComplete()
        {
            exitTween.Completed -= OnExitComplete;
            previousUI.gameObject.SetActive(false);
        }

        private void OnEntryComplete()
        {
            entryTween.Completed -= OnEntryComplete;
            swapButton.Selectable = true;
        }
    }
}
