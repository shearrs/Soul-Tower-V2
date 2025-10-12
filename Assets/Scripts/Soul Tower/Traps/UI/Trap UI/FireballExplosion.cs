using Shears;
using Shears.Tweens;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Traps.UI
{
    public class FireballExplosion : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Transform inner;
        [SerializeField] private Transform mid;
        [SerializeField] private Transform outer;

        [SerializeField] private MeshRenderer innerMesh;
        [SerializeField] private MeshRenderer midMesh;
        [SerializeField] private MeshRenderer outerMesh;

        private Material innerMat;
        private Material midMat;
        private Material outerMat;

        [Header("Settings")]
        [SerializeField] private float innerMax;
        [SerializeField] private float midMax;
        [SerializeField] private float outerMax;

        [Header("Animation Settings")]
        [SerializeField] private float extendDelay = 0f;
        [SerializeField] private TweenData extendTweenData;
        [SerializeField] private TweenData returnTweenData;

        private Tween tween1;
        private Tween tween2;
        private Tween tween3;

        private void Awake()
        {
            innerMat = Instantiate(innerMesh.material);
            midMat = Instantiate(midMesh.material);
            outerMat = Instantiate(outerMesh.material);

            innerMesh.material = innerMat;
            midMesh.material = midMat;
            outerMesh.material = outerMat;
        }
        private void Start()
        {
            tween1.Dispose();
            StopAllCoroutines();

            tween1 = inner.DoScaleLocalTween(new Vector3(innerMax, innerMax, innerMax), extendTweenData);
            tween2 = mid.DoScaleLocalTween(new Vector3(midMax, midMax, midMax), extendTweenData);
            tween3 = outer.DoScaleLocalTween(new Vector3(outerMax, outerMax, outerMax), extendTweenData);

            tween1.Completed += () => StartCoroutine(IEDelayTween());
        }

        private IEnumerator IEDelayTween()
        {
            yield return CoroutineUtil.WaitForSeconds(extendDelay);
            tween1.Dispose();
            tween2.Dispose();
            tween3.Dispose();

            void tweenColor(float t, Material mat, Color startColor, Color startEmission)
            {
                Color c = mat.color;
                c.a = 0;

                mat.SetColor("_EmissionColor", Color.LerpUnclamped(startEmission, Color.black, t));

                mat.color = Color.LerpUnclamped(startColor, c, t);
            }

            tween1 = TweenManager.DoTween((t) => tweenColor(t, innerMat, innerMat.color, innerMat.GetColor("_EmissionColor"))).WithLifetime(this);
            tween2 = TweenManager.DoTween((t) => tweenColor(t, midMat, midMat.color, midMat.GetColor("_EmissionColor"))).WithLifetime(this);
            tween3 = TweenManager.DoTween((t) => tweenColor(t, outerMat, outerMat.color, outerMat.GetColor("_EmissionColor"))).WithLifetime(this);
        }
    }
}
