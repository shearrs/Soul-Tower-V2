using Shears;
using Shears.Tweens;
using System.Collections;
using UnityEngine;

namespace SoulTower.Traps.UI
{
    public class SprinklerTrapAnimation : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private SprinklerTrap sprinklerTrap;
        [SerializeField] private Transform rotationHandle;
        [SerializeField] private Transform nozzle;
        [SerializeField] private Transform waterBubble;
        [SerializeField] private ParticleSystem spray1;
        [SerializeField] private ParticleSystem spray2;
        [SerializeField] private ParticleSystem ground;
        [SerializeField] private ParticleSystem cooldown1;
        [SerializeField] private ParticleSystem cooldown2;

        [Header("Settings")]
        [SerializeField] private float sprinklerRotationY;
        [SerializeField] private Vector3 bubbleScaleMin;
        [SerializeField] private Vector3 bubbleScaleMax;
        [SerializeField] private float bubbleScaleOffset;

        [Header("Animation Settings")]
        [SerializeField] private TweenData rotateTweenData;
        [SerializeField] private TweenData shakeTweenData;
        [SerializeField] private TweenData bubbleScaleUpTweenData;
        [SerializeField] private TweenData bubbleScaleDownTweenData;
        [SerializeField] private TweenData bubbleRotateTweenData1;
        [SerializeField] private TweenData bubbleRotateTweenData2;
        [SerializeField] private TweenData bubbleAmbientScale;

        private Tween tween; //rotator
        private Tween tween2; //bubble
        private Tween tween3; //shake

        private Tween tween4; //bubbleScaleAmbient

        private void OnEnable()
        {
            sprinklerTrap.Activated += OnSprinklerActivated;
        }

        private void OnDisable()
        {
            sprinklerTrap.Activated -= OnSprinklerActivated;
        }

        private void Start()
        {
            ScaleBubbleDown();
        }

        private void ScaleBubbleUp()
        {
            tween4.Dispose();
            tween4 = waterBubble.DoScaleLocalTween(new Vector3(waterBubble.localScale.x + bubbleScaleOffset, waterBubble.localScale.y + bubbleScaleOffset, waterBubble.localScale.z + bubbleScaleOffset), bubbleAmbientScale);
            tween4.Completed += ScaleBubbleDown;
        }

        private void ScaleBubbleDown()
        {
            tween4.Dispose();
            tween4 = waterBubble.DoScaleLocalTween(new Vector3(waterBubble.localScale.x - bubbleScaleOffset, waterBubble.localScale.y - bubbleScaleOffset, waterBubble.localScale.z - bubbleScaleOffset), bubbleAmbientScale);
            tween4.Completed += ScaleBubbleUp;
        }

        private void Update()
        {
            waterBubble.transform.Rotate(47f * Time.deltaTime, 0f, 0f);
            waterBubble.transform.Rotate(0f, 24f * Time.deltaTime, 0f);
            waterBubble.transform.Rotate(0f, 0f, 19f * Time.deltaTime);
        }

        private void OnSprinklerActivated(Trap _)
        {
            spray1.Play();
            spray2.Play();
            ground.Play();

            tween.Dispose();
            tween2.Dispose();
            tween3.Dispose();
            tween4.Dispose();

            tween = rotationHandle.DoRotateLocalTween(Quaternion.Euler(0f, sprinklerRotationY, 0f), true, rotateTweenData);
            tween2 = waterBubble.DoScaleLocalTween(bubbleScaleMin, bubbleScaleDownTweenData);
            tween3 = waterBubble.DoShakeTween(.025f, .05f, shakeTweenData);

            tween.Completed += DoRechargeAnimation;
        }

        private void DoRechargeAnimation()
        {
            cooldown1.Play();
            cooldown2.Play();

            tween.Dispose();
            tween2.Dispose();
            tween3.Dispose();

            rotationHandle.rotation = Quaternion.identity;

            tween2 = waterBubble.DoScaleLocalTween(bubbleScaleMax, bubbleScaleUpTweenData);
            tween2.Completed += ScaleBubbleDown;
        }
    }
}
