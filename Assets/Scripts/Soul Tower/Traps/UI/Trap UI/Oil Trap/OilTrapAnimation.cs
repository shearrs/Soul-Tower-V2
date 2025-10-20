using Shears;
using Shears.Tweens;
using System.Collections;
using UnityEngine;

namespace SoulTower.Traps.UI
{
    public class OilTrapAnimation : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private OilTrap oilTrap;
        [SerializeField] private Transform oilSlick;
        [SerializeField] private ParticleSystem[] particles;

        [Header("Animation Settings")]
        [SerializeField] private float evaporateDelay = 3f;

        [SerializeField] private TweenData evaporateTweenData;

        private Tween tween;

        private void OnEnable()
        {
            oilTrap.LitOnFire += OnOilTrapLitOnFire;
        }

        private void OnDisable()
        {
            oilTrap.LitOnFire -= OnOilTrapLitOnFire;
        }

        private void OnOilTrapLitOnFire()
        {
            foreach (var particle in particles)
                particle.Play();

            StartCoroutine(IEDelayTween());
        }

        private IEnumerator IEDelayTween()
        {
            yield return CoroutineUtil.WaitForSeconds(evaporateDelay);
            tween.Dispose();
            tween = oilSlick.DoScaleLocalTween(new Vector3(1f, 0f, 1f), evaporateTweenData);
            tween.Completed += DeleteTrap;
        }

        private void DeleteTrap()
        {
            var slot = oilTrap.GetComponentInParent<TrapSlot>();

            slot.RemoveTrap();
        }
    }
}
