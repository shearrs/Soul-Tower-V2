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

        [Header("Audio")]
        [SerializeField] private AudioSource trapAudio;
        [SerializeField] private AudioClip extendClip;
        [SerializeField] private AudioClip slamClip;
        [SerializeField] private AudioClip retractClip;


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

        private void OnRangeCalculated(TrapRangeDefinition def)
        {
            crusherScale = new(crusherScale.Min, def.Distance / pressModel.localScale.y);
            plateHeight = new(plateHeight.Min, def.Distance - 0.15f);
        }

        private void OnCrusherTrapActivated(Trap _)
        {
            tween0.Dispose();
            StopAllCoroutines();

            StartCoroutine(IEDelayParticle());
            StartCoroutine(PlayAudioSequence());

            tween0 = plate.DoMoveLocalTween(plateHeight.Max * Vector3.up, extendTweenData);
            tween1 = press.DoScaleLocalTween(press.transform.localScale.With(y: crusherScale.Max), extendTweenData);
            tween0.Completed += () => StartCoroutine(IEDelayTween());
        }

        private IEnumerator PlayAudioSequence()
        {
            trapAudio.clip = extendClip;
            trapAudio.Play();
            yield return CoroutineUtil.WaitForSeconds(extendClip.length);

            trapAudio.clip = slamClip;
            trapAudio.Play();
        }

        private IEnumerator IEDelayTween()
        {
            yield return CoroutineUtil.WaitForSeconds(extendDelay);

            trapAudio.clip = retractClip;
            trapAudio.Play();

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
