using Shears;
using Shears.Tweens;
using System;
using System.Collections;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class ClimberHook : MonoBehaviour
    {
        [SerializeField] private TrailRenderer ropeRenderer;
        [SerializeField] private TrailRenderer[] smearRenderers;
        [SerializeField] private GameObject hookModel;
        [SerializeField] private TweenData byebyeTweenData;
        [SerializeField] private TweenData byebyeVertTweenData;
        [SerializeField] private TweenData byebyeRotTweenData;

        private readonly StructTweenData tweenData = new(1.0f);
        private readonly StructTweenData throwRotateTweenData = new(0.1f);
        private readonly StructTweenData snapTweenData = new(0.3f, easingFunction: TweenEase.EaseOutBounce);

        private float delayDestroyAnim;

        private Vector3 snapPosition;
        private Vector3 snapDirection;

        private Tween tween; //vertical
        private Tween tween2; //horizontal
        private Tween tween3;
        private Tween tween4;

        public event Action ReachedDestination;

        public void Smear()
        {
            foreach (var renderer in smearRenderers)
                renderer.emitting = true;
        }

        public void Throw(Vector3 startPosition, Vector3 targetPosition, Vector3 snapPosition)
        {
            hookModel.transform.localPosition = new Vector3(-0.122000001f, 0.326999992f, 0.0599999987f);
            hookModel.transform.localRotation = Quaternion.Euler(new Vector3(354.990692f, 92.7285538f, 11.8595381f));
            hookModel.transform.localScale = new Vector3(0.663218379f, 0.663218379f, 0.663218379f);

            float dist = Vector3.Distance(targetPosition, startPosition);
            delayDestroyAnim = 1.4575f * dist;
            ropeRenderer.time = delayDestroyAnim;

            foreach (var renderer in smearRenderers)
            {
                renderer.emitting = false;
                renderer.enabled = false;
            }

            transform.SetParent(null);
            transform.position = startPosition;
            this.snapPosition = snapPosition;
            snapDirection = (targetPosition - snapPosition).normalized;

            transform.DoRotateTween(Quaternion.LookRotation(-snapDirection), true, throwRotateTweenData);
            var tween = transform.DoMoveTween(targetPosition, tweenData);
            tween.Completed += OnTweenComplete;

            ropeRenderer.emitting = true;
        }

        public IEnumerator DestroyAfterUse()
        {
            yield return CoroutineUtil.WaitForSeconds(delayDestroyAnim);

            tween = transform.DoMoveTween(transform.position + new Vector3(0f, -10f, 0f), byebyeVertTweenData);
            tween3 = transform.DoRotateTween(Quaternion.Euler(new Vector3(transform.rotation.x + 180, transform.rotation.y, transform.rotation.z)), true, byebyeTweenData);
            tween4 = transform.DoScaleLocalTween(Vector3.zero, byebyeTweenData);

            tween.Completed += () => Destroy(gameObject);
        }

        private void OnTweenComplete()
        {
            ropeRenderer.emitting = false;
            StartCoroutine(DestroyAfterUse());
            Vector3 lookDirection = Vector3.Lerp(transform.forward, Vector3.down, 0.35f);
            var rotation = Quaternion.LookRotation(lookDirection);

            transform.DoRotateTween(rotation, true, snapTweenData);
            var tween = transform.DoMoveTween(snapPosition, snapTweenData);
            tween.Completed += () => ropeRenderer.emitting = false;

            ReachedDestination?.Invoke();
        }

        private void OnDestroy()
        {
            tween.Dispose();
            tween2.Dispose();
            tween3.Dispose();
            tween4.Dispose();
        }
    }
}
