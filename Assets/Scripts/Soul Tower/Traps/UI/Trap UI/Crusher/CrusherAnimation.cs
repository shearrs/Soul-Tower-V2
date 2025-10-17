using Shears;
using Shears.Tweens;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Traps.UI
{
    public class CrusherAnimation : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private CruelCrusher crusherTrap;
        [SerializeField] private Transform crusher;
        [SerializeField] private Transform plate;

        [Header("Settings")]
        [SerializeField] private Range<float> crusherScale;
        [SerializeField] private Range<float> plateHeight;

        [Header("Animation Settings")]
        [SerializeField] private float extendDelay = 1f;
        [SerializeField] private TweenData extendTweenData;
        [SerializeField] private TweenData returnTweenData;

        private Tween tween;
        private Tween tween2;

        public void Start()
        {
            /*tween = plate.DoMoveLocalTween(plate.transform.localPosition + new Vector3(0f, plateHeight.Max, 0f), extendTweenData);
            tween2 = crusher.DoScaleLocalTween(new Vector3(crusher.transform.localScale.x, crusherScale.Max, crusher.transform.localScale.z), extendTweenData);
            tween.Completed += () => StartCoroutine(IEDelayTween());*/
        }

        private void OnEnable()
        {
            crusherTrap.Activated += OnCrusherTrapActivated;
        }

        private void OnDisable()
        {
            crusherTrap.Activated -= OnCrusherTrapActivated;
        }


        private void OnCrusherTrapActivated(Trap _)
        {
            tween.Dispose();
            StopAllCoroutines();

            tween = plate.DoMoveLocalTween(plate.transform.localPosition + new Vector3(0f, plateHeight.Max, 0f), extendTweenData);
            tween2 = crusher.DoScaleLocalTween(new Vector3(crusher.transform.localScale.x, crusherScale.Max, crusher.transform.localScale.z), extendTweenData);
            tween.Completed += () => StartCoroutine(IEDelayTween());
        }

        private IEnumerator IEDelayTween()
        {
            yield return CoroutineUtil.WaitForSeconds(extendDelay);

            tween.Dispose();
            tween = plate.DoMoveLocalTween(new Vector3(plate.transform.localPosition.x, plateHeight.Min, plate.transform.localPosition.z), returnTweenData);
            tween2 = crusher.DoScaleLocalTween(new Vector3(crusher.transform.localScale.x, crusherScale.Min, crusher.transform.localScale.z), returnTweenData);
        } 
    }
}
