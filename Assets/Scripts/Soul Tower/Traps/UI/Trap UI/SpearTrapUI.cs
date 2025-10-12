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
        [SerializeField] private List<SpearTrapSpear> spears;

        [Header("Animation Settings")]
        [SerializeField] private float extendDelay = 0.25f;
        [SerializeField] private TweenData extendTweenData;
        [SerializeField] private TweenData returnTweenData;

        private Tween tween;

        private void OnEnable()
        {
            spearTrap.Activated += OnSpearTrapActivated;
        }

        private void OnDisable()
        {
            spearTrap.Activated -= OnSpearTrapActivated;
        }

        private void OnSpearTrapActivated()
        {
            tween.Dispose();
            StopAllCoroutines();

            tween = TweenManager
                .DoTween((t) =>
                {
                    foreach (var spear in spears)
                        spear.SetPosition(t);
                }, extendTweenData)
                .WithLifetime(this);

            tween.Completed += () => StartCoroutine(IEDelayTween());
        }

        private IEnumerator IEDelayTween()
        {
            yield return CoroutineUtil.WaitForSeconds(extendDelay);

            tween.Dispose();

            tween = TweenManager
            .DoTween((t) =>
            {
                foreach (var spear in spears)
                    spear.SetPosition(1.0f - t);
            }, returnTweenData)
            .WithLifetime(this);
        }
    }
}
