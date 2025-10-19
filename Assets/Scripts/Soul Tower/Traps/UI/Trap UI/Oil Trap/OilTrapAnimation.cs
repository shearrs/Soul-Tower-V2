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
        [SerializeField] private Transform particlePos;
        [SerializeField] private GameObject particlePrefab;

        [Header("Animation Settings")]
        [SerializeField] private float evaporateDelay = 3f;

        [SerializeField] private TweenData evaporateTweenData;

        private Tween tween;

        private void OnEnable()
        {
            oilTrap.Activated += OnOilTrapActivated;
        }

        private void OnDisable()
        {
            oilTrap.Activated -= OnOilTrapActivated;
        }

        private void OnOilTrapActivated(Trap _)
        {
            GameObject particles = Instantiate(particlePrefab);
            particles.transform.position = particlePos.position;
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
            GameObject slot = oilTrap.transform.parent.gameObject.transform.parent.gameObject;
            if (slot.GetComponent<TrapSlot>() != null)
            {
                slot.GetComponent<TrapSlot>().RemoveTrap();
            }
        }
    }
}
