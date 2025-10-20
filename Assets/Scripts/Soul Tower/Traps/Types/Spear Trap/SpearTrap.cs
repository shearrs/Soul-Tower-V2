using Shears;
using Shears.HitDetection;
using System;
using UnityEngine;

namespace SoulTower.Traps
{
    [RequireComponent(typeof(Trap), typeof(TrapHitDeliverer))]
    public class SpearTrap : ManagedWrapper<Trap>
    {
        [SerializeField] private TrapRangeCalculator rangeCalculator;
        [SerializeField] private HitBox3D hitBox;
        [SerializeField] private TrapThreatArea threatArea;
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
            rangeCalculator.RangeCalculated += OnRangeCalculated;
        }

        private void OnDisable()
        {
            Trap.Activated -= OnActivated;
            hitTimer.Completed -= OnTimerEnd;
            hitDeliverer.HitBlocked -= OnHitBlocked;
            rangeCalculator.RangeCalculated -= OnRangeCalculated;
        }

        private void OnRangeCalculated(TrapRangeDefinition def)
        {
            Vector3 midpoint = Vector3.Lerp(transform.position, def.Point, 0.5f);

            threatArea.SetCenter(midpoint);
            threatArea.SetSize(threatArea.GetSize().With(y: def.Distance));
        }

        private void OnActivated(Trap _)
        {
            hitBox.enabled = true;

            hitTimer.Restart(hitDuration);
        }

        private void OnHitBlocked(HitData3D _)
        {
            hitBox.enabled = false;
            hitTimer.Stop();
            HitBlocked?.Invoke();
        }

        private void OnTimerEnd()
        {
            hitBox.enabled = false;
        }
    }
}
