using Shears;
using Shears.Signals;
using Shears.Tweens;
using UnityEngine;

namespace SoulTower.Currency
{
    public class DroppedSoul : MonoBehaviour
    {
        private const int SOUL_VALUE = 1;

        [Header("Components")]
        [SerializeField] private Transform innerSoul;
        [SerializeField] private Transform outerSoul;
        [SerializeField] private Transform soulGlow;
        [SerializeField] private ParticleSystem flame;
        [SerializeField] private ParticleSystem back;
        [SerializeField] private TweenData moveTweenData;
        [SerializeField] private TweenData floatTweenData;
        [SerializeField] private TweenData scaleTweenData;

        private Tween tween; //position
        private bool collected;

        public Vector3 CatalystLocation { get; set; }
        public int Value => SOUL_VALUE;

        public void Collect()
        {
            if (collected) 
                return;

            tween = transform.DoMoveTween(new Vector3(transform.position.x, transform.position.y + 0.4f, transform.position.z), floatTweenData);
            soulGlow.DoScaleLocalTween(new Vector3(2.100186f, 2.100186f, 2.100186f), scaleTweenData);
            tween.Completed += BeginMoveToCatalyst;

            collected = true;
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

            moveTweenData.Duration += Random.Range(-0.15f, 0.15f);
            tween = TweenManager.DoTween(orbitalTween, moveTweenData).WithLifetime(this);
            tween.Completed += BeginDestroy;
        }

        private void BeginDestroy()
        {
            SignalShuttle.Emit(new SoulCollectedSignal(this));

            tween.Dispose();
            flame.Stop();
            back.Stop();

            innerSoul.gameObject.SetActive(false);
            outerSoul.gameObject.SetActive(false);

            CoroutineUtil.DoAfter(() => Destroy(gameObject), 2.0f);
        }
    }
}
