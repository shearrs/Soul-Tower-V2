using Shears;
using Shears.Tweens;
using System.Collections;
using UnityEngine;

namespace SoulTower.Traps.UI
{
    public class DartTrapAnimation : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private DartTrap dartTrap;
        [SerializeField] private Transform head;
        [SerializeField] private Transform snout;

        [Header("Settings")]
        [SerializeField] private Range<float> headLocalYScale;
        [SerializeField] private Range<float> snoutLocalYScale;

        [Header("Animation Settings")]
        [SerializeField] private float extendDelay = 0f;
        [SerializeField] private TweenData extendTweenData;
        [SerializeField] private TweenData returnTweenData;

        private Tween tween;
        private Tween tween2;

        private void OnEnable()
        {
            dartTrap.Activated += OnDartTrapActivated;
        }

        private void OnDisable()
        {
            dartTrap.Activated -= OnDartTrapActivated;
        }

        private void OnDartTrapActivated(Trap _)
        {
            dartTrap.SpawnDart();

            tween.Dispose();
            tween2.Dispose();
            StopAllCoroutines();

            tween = head.DoScaleLocalTween(new Vector3(head.transform.localScale.x, headLocalYScale.Max, head.transform.localScale.z), extendTweenData);
            tween2 = snout.DoScaleLocalTween(new Vector3(snout.transform.localScale.x, snoutLocalYScale.Max, snout.transform.localScale.z), extendTweenData);
            tween.Completed += () => StartCoroutine(IEDelayTween());
        }

        private IEnumerator IEDelayTween()
        {
            yield return CoroutineUtil.WaitForSeconds(extendDelay);
            tween.Dispose();
            tween2.Dispose();

            tween = head.DoScaleLocalTween(new Vector3(head.transform.localScale.x, headLocalYScale.Min, head.transform.localScale.z), returnTweenData);
            tween2 = snout.DoScaleLocalTween(new Vector3(snout.transform.localScale.x, snoutLocalYScale.Min, snout.transform.localScale.z), returnTweenData);
        }
    }
}
