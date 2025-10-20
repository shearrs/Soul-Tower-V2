using Shears.Tweens;
using UnityEngine;

namespace SoulTower.Enemies.UI
{
    public class DefenderShieldGraphics : MonoBehaviour
    {
        private static readonly Vector3 HIT_SCALE = new(1.25f, 1.25f, 1f);

        [SerializeField] private Transform shieldModel;
        [SerializeField] private DefenderShieldHitReceiver hitReceiver;

        private readonly StructTweenData blockTweenData = new(0.1f, easingFunction: TweenEase.EaseOutBack);
        private Tween blockTween;
        private Tween shakeTween;

        private void OnEnable()
        {
            hitReceiver.HitBlocked += OnHitBlocked;
        }

        private void OnDisable()
        {
            hitReceiver.HitBlocked -= OnHitBlocked;
            blockTween.Dispose();
            shakeTween.Dispose();
        }

        private void OnHitBlocked()
        {
            if (!shakeTween.IsPlaying)
                shakeTween = shieldModel.DoShakeTween(0.05f, shakeDelay: 0.05f, data: new StructTweenData(0.11f));

            blockTween.Dispose();
            blockTween = shieldModel.DoScaleLocalTween(HIT_SCALE, blockTweenData);
            blockTween.Completed += () => blockTween = shieldModel.DoScaleLocalTween(Vector3.one, new StructTweenData(0.2f));
        }
    }
}
