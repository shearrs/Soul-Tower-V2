using Shears;
using Shears.HitDetection;
using System;
using UnityEngine;

namespace SoulTower.Traps
{
    [RequireComponent(typeof(Trap))]
    public class SprinklerTrap : ManagedWrapper<Trap>
    {
        [SerializeField] private TrapRangeCalculator rangeCalculator;
        [SerializeField] private HitBox3D hitBox;
        [SerializeField, Min(0)] private float hitDuration = .15f;

        private readonly Timer hitTimer = new();

        private Trap Trap => TypedWrappedValue;

        public event Action<Trap> Activated { add => TypedWrappedValue.Activated += value; remove => TypedWrappedValue.Activated -= value; }

        private void OnEnable()
        {
            rangeCalculator.RangeCalculated += OnRangeCalculated;
            Trap.Activated += OnActivated;
            hitTimer.Completed += OnTimerEnd;
        }

        private void OnDisable()
        {
            rangeCalculator.RangeCalculated -= OnRangeCalculated;
            Trap.Activated -= OnActivated;
            hitTimer.Completed -= OnTimerEnd;
        }

        private void OnRangeCalculated(TrapRangeDefinition def)
        {
            Vector3 midpoint = Vector3.Lerp(transform.position, def.Point, 0.5f);
            hitBox.WorldCenter = midpoint;
            hitBox.Size = hitBox.Size.With(y: def.Distance);
        }

        private void OnActivated(Trap _)
        {
            hitBox.enabled = true;

            hitTimer.Restart(hitDuration);
        }

        private void OnTimerEnd()
        {
            hitBox.enabled = false;
        }
    }
}
