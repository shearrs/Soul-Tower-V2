using Shears.Tweens;
using System;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class ClimberHook : MonoBehaviour
    {
        [SerializeField] private TrailRenderer ropeRenderer;
        [SerializeField] private TrailRenderer[] smearRenderers;

        private readonly StructTweenData tweenData = new(1.0f);
        private readonly StructTweenData throwRotateTweenData = new(0.1f);
        private readonly StructTweenData snapTweenData = new(0.3f, easingFunction: TweenEase.EaseOutBounce);

        private Vector3 snapPosition;
        private Vector3 snapDirection;

        public event Action ReachedDestination;

        public void Smear()
        {
            foreach (var renderer in smearRenderers)
                renderer.emitting = true;
        }

        public void Throw(Vector3 startPosition, Vector3 targetPosition, Vector3 snapPosition)
        {
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

        public void DestroyAfterUse()
        {
            Destroy(gameObject);
        }

        private void OnTweenComplete()
        {
            Vector3 lookDirection = Vector3.Lerp(transform.forward, Vector3.down, 0.35f);
            var rotation = Quaternion.LookRotation(lookDirection);

            transform.DoRotateTween(rotation, false, snapTweenData);
            var tween = transform.DoMoveTween(snapPosition, snapTweenData);
            tween.Completed += () => ropeRenderer.emitting = false;

            ReachedDestination?.Invoke();
        }
    }
}
