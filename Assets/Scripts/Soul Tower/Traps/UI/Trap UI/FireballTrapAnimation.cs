using Shears;
using Shears.Tweens;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Traps.UI
{
    public class FireballTrapAnimation : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private FireballTrap fireballTrap;
        [SerializeField] private Transform headTop;
        [SerializeField] private Transform headBottom;

        [Header("Settings")]
        [SerializeField] private Range<float> headTopX;
        [SerializeField] private Range<float> headBottomX;

        [Header("Animation Settings")]
        [SerializeField] private float extendDelay = .75f;
        [SerializeField] private TweenData extendTweenData;
        [SerializeField] private TweenData returnTweenData;

        private Tween tween;
        private Tween tween2;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }
        private void OnEnable()
        {
            fireballTrap.Activated += OnFireballTrapActivated;
        }

        private void OnDisable()
        {
            fireballTrap.Activated -= OnFireballTrapActivated;
        }

        private void OnFireballTrapActivated(Trap _)
        {
            tween.Dispose();
            StopAllCoroutines();

            tween = headTop.DoMoveLocalTween(new Vector3(headTopX.Max, headTop.transform.localPosition.y, headTop.transform.localPosition.z), extendTweenData);
            tween2 = headBottom.DoMoveLocalTween(new Vector3(headBottomX.Min, headBottom.transform.localPosition.y, headBottom.transform.localPosition.z), extendTweenData);
            tween.Completed += () => StartCoroutine(IEDelayTween());
        }

        private IEnumerator IEDelayTween()
        {
            yield return CoroutineUtil.WaitForSeconds(extendDelay);

            tween.Dispose();
            tween = headTop.DoMoveLocalTween(new Vector3(headTopX.Min, headTop.transform.localPosition.y, headTop.transform.localPosition.z), returnTweenData);
            tween2 = headBottom.DoMoveLocalTween(new Vector3(headBottomX.Max, headBottom.transform.localPosition.y, headBottom.transform.localPosition.z), returnTweenData);
        }
    }
}
