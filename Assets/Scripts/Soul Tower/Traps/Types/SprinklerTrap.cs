using Shears;
using Shears.HitDetection;
using System;
using UnityEngine;

namespace SoulTower.Traps
{
    [RequireComponent(typeof(Trap))]
    public class SprinklerTrap : ManagedWrapper<Trap>
    {
        [SerializeField] private HitBody3D hitBody;
        [SerializeField, Min(0)] private float hitDuration = .15f;

        private readonly Timer hitTimer = new();

        private Trap Trap => TypedWrappedValue;

        public event Action<Trap> Activated { add => TypedWrappedValue.Activated += value; remove => TypedWrappedValue.Activated -= value; }

        private void OnEnable()
        {
            Trap.Activated += OnActivated;
            hitTimer.Completed += OnTimerEnd;
        }

        private void OnDisable()
        {
            Trap.Activated -= OnActivated;
            hitTimer.Completed -= OnTimerEnd;
        }

        private void OnActivated(Trap _)
        {
            hitBody.enabled = true;

            hitTimer.Restart(hitDuration);
        }

        private void OnTimerEnd()
        {
            hitBody.enabled = false;
        }
    }
}
