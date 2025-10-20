using Shears;
using Shears.Logging;
using System;
using UnityEngine;

namespace SoulTower.Traps
{
    public readonly struct TrapRangeDefinition
    {
        private readonly float distance;
        private readonly Vector3 point;

        public readonly float Distance => distance;
        public readonly Vector3 Point => point;

        public TrapRangeDefinition(float distance, Vector3 point)
        {
            this.distance = distance;
            this.point = point;
        }
    }

    [RequireComponent(typeof(Trap))]
    public class TrapRangeCalculator : SHMonoBehaviourLogger
    {
        [SerializeField] private float maxRange = 6.0f;

        public event Action<TrapRangeDefinition> RangeCalculated;

        private void Awake()
        {
            CoroutineUtil.DoDeferred(() =>
            {
                if (!Physics.Raycast(transform.position, transform.up, out var hit, maxRange, LayerMask.GetMask("Tower")))
                {
                    Log("Could not find tower surface.", SHLogLevels.Verbose);

                    RangeCalculated?.Invoke(new(maxRange, transform.position + transform.up * maxRange));
                    return;
                }

                RangeCalculated?.Invoke(new(hit.distance, hit.point));
            });
        }
    }
}
