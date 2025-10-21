using Shears;
using Shears.Tweens;
using SoulTower.Towers;
using System.Collections;
using UnityEngine;

namespace SoulTower.Traps.UI
{
    public class FreezeBreezeTrapAnimation : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private FreezeBreeze freezeBreeze;
        [SerializeField] private TrapRangeCalculator rangeCalculator;
        [SerializeField] private Transform arm1;
        [SerializeField] private Transform arm2;
        [SerializeField] private ParticleSystem frost;
        [SerializeField] private ParticleSystem ice;
        [SerializeField] private ParticleSystem snow;
        [SerializeField] private MeshRenderer gem;
        [SerializeField] private MeshRenderer gem2;

        private Material gemMat;
        private Material gemMat2;

        [Header("Settings")]
        [SerializeField] private Range<float> arm1Rotation;
        [SerializeField] private Range<float> arm2Rotation;

        [Header("Animation Settings")]
        [SerializeField] private float extendDelay = .2f;
        [SerializeField] private TweenData extendTweenData;
        [SerializeField] private TweenData returnTweenData;
        [SerializeField] private TweenData emissiveTweenData;

        [Header("Audio")]
        [SerializeField] private AudioSource trapAudio;
        [SerializeField] private AudioClip fireClip;

        private Tween tween;
        private Tween tween2;
        private Tween tween3;
        private Tween tween4;

        private readonly int EMISSION_ID = Shader.PropertyToID("_EmissionColor");

        private void Awake()
        {
            gemMat = Instantiate(gem.material);
            gemMat2 = Instantiate(gem2.material);
            gem.material = gemMat;
            gem2.material = gemMat2;
        }

        private void OnEnable()
        {
            freezeBreeze.Activated += OnFreezeBreezeActivated;
            rangeCalculator.RangeCalculated += OnRangeCalculated;
        }

        private void OnDisable()
        {
            freezeBreeze.Activated -= OnFreezeBreezeActivated;
            rangeCalculator.RangeCalculated -= OnRangeCalculated;
        }

        private void OnRangeCalculated(TrapRangeDefinition def)
        {
            ice.transform.position = def.Point + Vector3.up * 0.01f;
        }

        private void OnFreezeBreezeActivated(Trap _)
        {
            frost.Play();
            snow.Play();

            trapAudio.clip = fireClip;
            trapAudio.Play();

            GameObject trapSlot = freezeBreeze.transform.parent.gameObject.transform.parent.gameObject;
            if(trapSlot.GetComponent<Tile>() != null)
            {
                if(trapSlot.GetComponent<Tile>().Type == TileType.Ceiling)
                {
                    ice.Play();
                }
            }

            tween.Dispose();
            tween2.Dispose();
            tween3.Dispose();
            tween4.Dispose();
            StopAllCoroutines();

            tween = arm1.DoRotateLocalTween(Quaternion.Euler(0f, 0f, arm1Rotation.Min), true, extendTweenData);
            tween2 = arm2.DoRotateLocalTween(Quaternion.Euler(0f, 0f, arm2Rotation.Max), true, extendTweenData);
            tween3 = gemMat.DoEmissionTween(gemMat.GetColor(EMISSION_ID) * 8f, emissiveTweenData);
            tween4 = gemMat2.DoEmissionTween(gemMat2.GetColor(EMISSION_ID) * 8f, emissiveTweenData);

            tween.Completed += () => StartCoroutine(IEDelayTween());
        }

        private IEnumerator IEDelayTween()
        {
            yield return CoroutineUtil.WaitForSeconds(extendDelay);
            tween.Dispose();
            tween2.Dispose();
            tween3.Dispose();

            tween = arm1.DoRotateLocalTween(Quaternion.Euler(0f, 0f, arm1Rotation.Max), true, returnTweenData);
            tween2 = arm2.DoRotateLocalTween(Quaternion.Euler(0f, 0f, arm2Rotation.Min), true, returnTweenData);
            tween3 = gemMat.DoEmissionTween(gemMat.GetColor(EMISSION_ID), emissiveTweenData);
            tween4 = gemMat2.DoEmissionTween(gemMat2.GetColor(EMISSION_ID), emissiveTweenData);
        }
    }
}
