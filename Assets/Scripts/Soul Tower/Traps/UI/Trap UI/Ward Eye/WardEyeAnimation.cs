using NUnit.Framework;
using Shears;
using Shears.Tweens;
using System.Collections;
using UnityEngine;

namespace SoulTower.Traps.UI
{
    public class WardEyeAnimation : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private WardEye eyeTrap;
        [SerializeField] private Transform eye;

        [Header("Settings")]
        public float[] Xrots = new float[3];
        public float[] Yrots = new float[3];

        [Header("Animation Settings")]
        [SerializeField] private float extendDelay = 0f;
        [SerializeField] private Range<float> delayRange;
        [SerializeField] private TweenData idleTweenData;
        [SerializeField] private TweenData quickCenterTweenData;
        [SerializeField] private TweenData switchTweenData;
        [SerializeField] private TweenData alertTweenData;

        private Tween tween;

        private int alertSwitchState = 1;

        void Start()
        {
            tween.Dispose();
            StopAllCoroutines();

            tween = eye.DoRotateLocalTween(Quaternion.Euler(new Vector3(Xrots[Random.Range(0, 3)], Yrots[Random.Range(0, 3)], 0f)), true, idleTweenData);
            tween.Completed += () => StartCoroutine(IEDelayTween());
        }

        private void OnEnable()
        {
            eyeTrap.Activated += OnWardTrapActivated;
        }

        private void OnDisable()
        {
            eyeTrap.Activated -= OnWardTrapActivated;
        }

        private IEnumerator IEDelayTween()
        {
            extendDelay = Random.Range(delayRange.Min, delayRange.Max);
            yield return CoroutineUtil.WaitForSeconds(extendDelay);
            tween.Dispose();
            StopAllCoroutines();

            tween = eye.DoRotateLocalTween(Quaternion.Euler(new Vector3(Xrots[Random.Range(0, 3)], Yrots[Random.Range(0, 3)], 0f)), true, idleTweenData);
            tween.Completed += () => StartCoroutine(IEDelayTween());
        }

        private void OnWardTrapActivated(Trap _)
        {
            tween.Dispose();
            StopAllCoroutines();

            tween = eye.DoRotateLocalTween(Quaternion.identity, true, quickCenterTweenData);
            tween.Completed += DoAlertSwitch;
        }

        private void DoAlertSwitch()
        {
            tween.Dispose();
            StopAllCoroutines();

            alertSwitchState = 1;
            tween = eye.DoRotateLocalTween(Quaternion.Euler(new Vector3(180f, 0f, 0f)), true, switchTweenData);
            tween.Completed += DoAlertShake;
        }

        private void DoAlertShake()
        {
            tween.Dispose();
            StopAllCoroutines();

            tween = eye.DoRotateLocalTween(Quaternion.Euler(new Vector3(180f, 20f * alertSwitchState, 0f)), true, alertTweenData);
            alertSwitchState *= -1;
            tween.Completed += DoAlertShake;
        }

        private void DoReverseSwitchOne()
        {
            tween.Dispose();
            StopAllCoroutines();
            tween = eye.DoRotateLocalTween(Quaternion.Euler(new Vector3(180f, 0f, 0f)), true, switchTweenData);
            tween.Completed += DoReverseSwitchTwo;
        }

        private void DoReverseSwitchTwo()
        {
            tween.Dispose();
            StopAllCoroutines();
            tween = eye.DoRotateLocalTween(Quaternion.identity, true, switchTweenData);
            tween.Completed += () => StartCoroutine(IEDelayTween());
        }
    }
}
