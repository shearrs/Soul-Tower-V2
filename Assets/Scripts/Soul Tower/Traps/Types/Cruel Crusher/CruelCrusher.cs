using Shears;
using Shears.HitDetection;
using System;
using UnityEngine;

namespace SoulTower.Traps
{
    [RequireComponent(typeof(Trap))]
    public class CruelCrusher : ManagedWrapper<Trap>
    {
        [SerializeField] private HitBody3D hitBody;
        [SerializeField, Min(0)] private float hitDuration = .15f;

        private readonly Timer hitTimer = new();
        private Trap trap;

        public event Action Activated { add => trap.Activated += value; remove => trap.Activated -= value; }

        private void Awake()
        {
            trap = GetComponent<Trap>();
        }

        private void OnEnable()
        {
            trap.Activated += OnActivated;
            hitTimer.Completed += OnTimerEnd;
        }

        private void OnDisable()
        {
            trap.Activated -= OnActivated;
            hitTimer.Completed -= OnTimerEnd;
        }

        private void OnActivated()
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
