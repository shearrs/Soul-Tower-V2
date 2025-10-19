using Shears;
using Shears.Tweens;
using System.Collections;
using UnityEngine;

namespace SoulTower.Traps.UI
{
    public class CrusherAnimation : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private CruelCrusher crusherTrap;
        [SerializeField] private TrapRangeCalculator rangeCalculator;
        [SerializeField] private Transform press;
        [SerializeField] private Transform pressModel;
        [SerializeField] private Transform plate;
        [SerializeField] private ParticleSystem particle;

        [Header("Settings")]
        [SerializeField] private Range<float> crusherScale;
        [SerializeField] private Range<float> plateHeight;

        [Header("Animation Settings")]
        [SerializeField] private float extendDelay = 1f;
        [SerializeField] private float particleDelay = 1f;
        [SerializeField] private TweenData extendTweenData;
        [SerializeField] private TweenData returnTweenData;

        private Tween tween0;
        private Tween tween1;

        private void OnEnable()
        {
            rangeCalculator.RangeCalculated += OnRangeCalculated;
            crusherTrap.Activated += OnCrusherTrapActivated;
        }

        private void OnDisable()
        {
            rangeCalculator.RangeCalculated -= OnRangeCalculated;
            crusherTrap.Activated -= OnCrusherTrapActivated;
        }

        private void OnRangeCalculated(RaycastHit hit)
        {
            crusherScale = new(crusherScale.Min, hit.distance / pressModel.localScale.y);
            plateHeight = new(plateHeight.Min, hit.distance - 0.15f);
        }

        private void OnCrusherTrapActivated(Trap _)
        {
            tween0.Dispose();
            StopAllCoroutines();

            StartCoroutine(IEDelayParticle());

            tween0 = plate.DoMoveLocalTween(plateHeight.Max * Vector3.up, extendTweenData);
            tween1 = press.DoScaleLocalTween(press.transform.localScale.With(y: crusherScale.Max), extendTweenData);
            tween0.Completed += () => StartCoroutine(IEDelayTween());
        }

        private IEnumerator IEDelayTween()
        {
            yield return CoroutineUtil.WaitForSeconds(extendDelay);

            tween0.Dispose();
            tween1.Dispose();
            tween0 = plate.DoMoveLocalTween(plateHeight.Min * Vector3.up, returnTweenData);
            tween1 = press.DoScaleLocalTween(press.transform.localScale.With(y: crusherScale.Min), returnTweenData);
        } 

        private IEnumerator IEDelayParticle()
        {
            yield return CoroutineUtil.WaitForSeconds(particleDelay);

            particle.Play();
        }
    }
}
