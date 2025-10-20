using Shears;
using UnityEngine;

namespace SoulTower.Traps.UI
{
    public class DartProjectileAnimation : MonoBehaviour
    {
        private const float DELAY_DURATION = 1.0f;

        [SerializeField] private DartProjectile dart;
        [SerializeField] private ParticleSystem[] particles;

        private readonly Timer delayTimer = new(DELAY_DURATION);

        private void OnEnable()
        {
            dart.LitOnFire += OnDartFireLit;
            dart.Destroyed += OnDartDestroyed;
            delayTimer.Completed += OnDelayTimerCompleted;
        }

        private void OnDisable()
        {
            dart.LitOnFire -= OnDartFireLit;
            dart.Destroyed -= OnDartDestroyed;
            delayTimer.Completed -= OnDelayTimerCompleted;
            delayTimer.Stop();
        }

        private void OnDartFireLit()
        {
            foreach (var particle in particles)
                particle.Play();
        }

        private void OnDartDestroyed()
        {
            foreach (var particle in particles)
                particle.Stop();

            transform.SetParent(null);
            delayTimer.Start();
        }

        private void OnDelayTimerCompleted()
        {
            Destroy(gameObject);
        }
    }
}
