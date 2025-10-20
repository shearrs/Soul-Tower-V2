using Shears;
using Shears.Tweens;
using SoulTower.HitDetection;
using UnityEngine;

namespace SoulTower.Enemies.UI
{
    /// <summary>
    /// Must be attached to the enemy's model.
    /// </summary>
    public class EnemySquasher : MonoBehaviour
    {
        private const float DESTROY_DELAY = 1.0f;
        private static readonly Vector3 SQUASH_SCALE = new(1.5f, 0.1f, 1.5f);

        [SerializeField] private EnemyHitReceiver hitReceiver;

        private bool hasBeenSquashed = false;
        private EnemyModel model;
        private Tween squashTween;

        private readonly StructTweenData squashTweenData = new(0.175f, easingFunction: TweenEase.EaseOutBack);

        private void Awake()
        {
            model = GetComponent<EnemyModel>();
        }

        private void OnEnable()
        {
            hitReceiver.DamageDataReceived += OnHitReceived;
        }

        private void OnDisable()
        {
            hitReceiver.DamageDataReceived -= OnHitReceived;
        }

        private void OnHitReceived(DamageData data)
        {
            if (data.Type != DamageType.Crushing || hasBeenSquashed)
                return;

            hasBeenSquashed = true;
            model.transform.parent = null;
            var enemyAnimator = model.Animator;
            var realAnimator = model.Animator.Animator;
            Destroy(model);
            Destroy(enemyAnimator);
            Destroy(realAnimator);
            squashTween = transform.DoScaleLocalTween(SQUASH_SCALE, squashTweenData);
            squashTween.Completed += OnSquashComplete;
        }

        private void OnSquashComplete()
        {
            CoroutineUtil.DoAfter(() =>
            {
                squashTween = transform.DoScaleLocalTween(Vector3.zero, new StructTweenData(DESTROY_DELAY));
                squashTween.Completed += () => Destroy(gameObject);
            }, Random.Range(2.0f, 2.5f));
        }
    }
}
