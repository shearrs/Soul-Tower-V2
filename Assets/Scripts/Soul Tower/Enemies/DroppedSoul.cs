using Shears;
using Shears.Tweens;
using SoulTower.Towers;
using System.Collections;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class DroppedSoul : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Transform innerSoul;
        [SerializeField] private Transform outerSoul;
        [SerializeField] private Transform soulGlow;
        [SerializeField] private ParticleSystem flame;
        [SerializeField] private ParticleSystem back;

        public Vector3 CatalystLocation;

        [SerializeField] private TweenData moveTweenData;
        [SerializeField] private TweenData floatTweenData;
        [SerializeField] private TweenData scaleTweenData;

        private bool collected;

        private Tween tween; //position
        private Tween tween2; //glowScaleLerp


        public void Collect()
        {
            if (collected) return;
            tween = transform.DoMoveTween(new Vector3(transform.position.x, transform.position.y + 0.4f, transform.position.z), floatTweenData);
            tween2 = soulGlow.DoScaleLocalTween(new Vector3(2.100186f, 2.100186f, 2.100186f), scaleTweenData);
            tween.Completed += BeginMoveToCatalyst;
            collected = true;
        }
        
        void Start()
        {
            /*tween = transform.DoMoveTween(new Vector3(transform.position.x, transform.position.y + 0.4f, transform.position.z), floatTweenData);
            tween.Completed += BeginMoveToCatalyst;*/
        }

        void Update()
        {
            if (innerSoul.gameObject.activeInHierarchy)
            {
                innerSoul.transform.Rotate(new Vector3(0f, 200f * Time.deltaTime, 0f));
                outerSoul.transform.Rotate(new Vector3(0f, -200f * Time.deltaTime, 0f));
            }
        }

        private void BeginMoveToCatalyst()
        {
            tween.Dispose();
            Vector3 startPos = transform.position;

            void orbitalTween(float t)
            {
                transform.position = Vector3.SlerpUnclamped(startPos, CatalystLocation, t);
            }

            tween = TweenManager.DoTween(orbitalTween, moveTweenData).WithLifetime(this);
            tween.Completed += BeginDestroy;
        }

        private void BeginDestroy()
        {
            tween.Dispose();
            flame.Stop();
            back.Stop();
            innerSoul.gameObject.SetActive(false);
            outerSoul.gameObject.SetActive(false);
            StartCoroutine(DelayDestroy());
        }

        private IEnumerator DelayDestroy()
        {
            yield return CoroutineUtil.WaitForSeconds(2f);
            Destroy(gameObject);
        }
    }
}
