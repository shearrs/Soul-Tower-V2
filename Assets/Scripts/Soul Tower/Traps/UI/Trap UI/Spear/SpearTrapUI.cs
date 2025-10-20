using Shears;
using Shears.Tweens;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Traps.UI
{
    public class SpearTrapUI : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private SpearTrap spearTrap;
        [SerializeField] private TrapRangeCalculator rangeCalculator;
        [SerializeField] private List<SpearTrapSpear> spears;

        [Header("Animation Settings")]
        [SerializeField] private float extendDelay = 0.25f;
        [SerializeField] private TweenData extendTweenData;
        [SerializeField] private TweenData returnTweenData;

        private float currentSpearPosition = 0.0f;
        private Tween tween;

        private void OnEnable()
        {
            spearTrap.Activated += OnSpearTrapActivated;
            spearTrap.HitBlocked += OnHitBlocked;
            rangeCalculator.RangeCalculated += OnRangeCalculated;
        }

        private void OnDisable()
        {
            spearTrap.Activated -= OnSpearTrapActivated;
            spearTrap.HitBlocked -= OnHitBlocked;
            rangeCalculator.RangeCalculated -= OnRangeCalculated;
        }

        private void OnRangeCalculated(TrapRangeDefinition def)
        {
            foreach (var spear in spears)
                spear.EndScale = (0.5f * def.Distance) - 0.2f;
        }

        private void OnHitBlocked()
        {
            tween.Dispose();
            StopAllCoroutines();
            StartCoroutine(IEDelayTween());
        }

        private void OnSpearTrapActivated(Trap _)
        {
            tween.Dispose();
            StopAllCoroutines();

            tween = TweenManager
                .DoTween((t) =>
                {
                    currentSpearPosition = t;

                    foreach (var spear in spears)
                        spear.SetPosition(currentSpearPosition);
                }, extendTweenData)
                .WithLifetime(this);

            tween.Completed += () => StartCoroutine(IEDelayTween());
        }

        private IEnumerator IEDelayTween()
        {
            yield return CoroutineUtil.WaitForSeconds(extendDelay);

            tween.Dispose();

            float start = currentSpearPosition;
            float end = 0.0f;

            tween = TweenManager
            .DoTween((t) =>
            {
                float pos = Mathf.LerpUnclamped(start, end, t);

                foreach (var spear in spears)
                    spear.SetPosition(pos);
            }, returnTweenData)
            .WithLifetime(this);
        }
    }
}
