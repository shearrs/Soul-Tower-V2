using Shears;
using Shears.HitDetection;
using System;
using UnityEngine;

namespace SoulTower.Traps
{
    [RequireComponent(typeof(Trap))]
    public class ShockSurface : ManagedWrapper<Trap>
    {
        private const float ACTIVE_DURATION = 2.0f;

        [SerializeField] private HitBody3D hitbody;

        private readonly Timer activeTimer = new(ACTIVE_DURATION);

        public event Action<Trap> Activated { add => TypedWrappedValue.Activated += value; remove => TypedWrappedValue.Activated -= value; }

        private void OnEnable()
        {
            Activated += OnActivated;
            activeTimer.Completed += OnActiveTimerCompleted;
        }

        private void OnDisable()
        {
            Activated -= OnActivated;
            activeTimer.Completed -= OnActiveTimerCompleted;

            activeTimer.Stop();
        }

        private void OnActivated(Trap _)
        {
            activeTimer.Restart();
            hitbody.enabled = true;
        }

        private void OnActiveTimerCompleted()
        {
            hitbody.enabled = false;
        }
    }
}
