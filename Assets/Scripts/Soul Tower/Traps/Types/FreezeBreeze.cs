using Shears;
using Shears.HitDetection;
using System;
using UnityEngine;

namespace SoulTower.Traps
{
    [RequireComponent(typeof(Trap))]
    public class FreezeBreeze : ManagedWrapper<Trap>
    {
        [SerializeField] private TrapRangeCalculator rangeCalculator;
        [SerializeField] private HitBox3D hitBox;

        private readonly Timer activeTimer = new(2f);

        public event Action<Trap> Activated { add => TypedWrappedValue.Activated += value; remove => TypedWrappedValue.Activated -= value; }

        private void OnEnable()
        {
            Activated += StartFrost;
            activeTimer.Completed += EndFrost;
            rangeCalculator.RangeCalculated += OnRangeCalculated;
        }

        private void OnDisable()
        {
            Activated -= StartFrost;
            activeTimer.Completed -= EndFrost;
            rangeCalculator.RangeCalculated -= OnRangeCalculated;
        }

        private void OnRangeCalculated(TrapRangeDefinition def)
        {
            hitBox.WorldCenter = Vector3.Lerp(transform.position, def.Point, 0.5f);
            hitBox.Size = hitBox.Size.With(y: def.Distance);
        }

        private void StartFrost(Trap _)
        {
            hitBox.enabled = true;
            activeTimer.Start();
        }

        private void EndFrost()
        {
            hitBox.enabled = false;
        }
    }
}
