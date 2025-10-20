using Shears;
using Shears.HitDetection;
using System;
using UnityEngine;

namespace SoulTower.Traps
{
    [RequireComponent(typeof(Trap))]
    public class CruelCrusher : ManagedWrapper<Trap>
    {
        [SerializeField] private TrapRangeCalculator rangeCalculator;
        [SerializeField] private HitBox3D hitBox;
        [SerializeField] private TrapThreatArea threatArea;
        [SerializeField, Min(0)] private float hitDuration = 3.0f;

        private readonly Timer hitTimer = new();
        private Trap trap;

        public event Action<Trap> Activated { add => trap.Activated += value; remove => trap.Activated -= value; }

        private void Awake()
        {
            trap = GetComponent<Trap>();
        }

        private void OnEnable()
        {
            trap.Activated += OnActivated;
            hitTimer.Completed += OnTimerEnd;
            rangeCalculator.RangeCalculated += OnRangeCalculated;
        }

        private void OnDisable()
        {
            trap.Activated -= OnActivated;
            hitTimer.Completed -= OnTimerEnd;
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

        private void OnTimerEnd()
        {
            hitBox.enabled = false;
        }
    }
}
