using Shears;
using Shears.HitDetection;
using System;
using UnityEngine;

namespace SoulTower.Traps
{
    [RequireComponent(typeof(Trap), typeof(TrapHitDeliverer))]
    public class SpearTrap : ManagedWrapper<Trap>
    {
        [SerializeField] private HitBody3D hitBody;
        [SerializeField, Min(0)] private float hitDuration = .15f;

        private readonly Timer hitTimer = new();

        private TrapHitDeliverer hitDeliverer;

        private Trap Trap => TypedWrappedValue;

        public event Action<Trap> Activated { add => Trap.Activated += value; remove => Trap.Activated -= value; }
        public event Action HitBlocked;

        private void Awake()
        {
            hitDeliverer = GetComponent<TrapHitDeliverer>();

        }

        private void OnEnable()
        {
            Trap.Activated += OnActivated;
            hitTimer.Completed += OnTimerEnd;
            hitDeliverer.HitBlocked += OnHitBlocked;
        }

        private void OnDisable()
        {
            Trap.Activated -= OnActivated;
            hitTimer.Completed -= OnTimerEnd;
            hitDeliverer.HitBlocked -= OnHitBlocked;
        }

        private void OnActivated(Trap _)
        {
            hitBody.enabled = true;

            hitTimer.Restart(hitDuration);
        }

        private void OnHitBlocked(HitData3D _)
        {
            hitBody.enabled = false;
            hitTimer.Stop();
            HitBlocked?.Invoke();
        }

        private void OnTimerEnd()
        {
            hitBody.enabled = false;
        }
    }
}
