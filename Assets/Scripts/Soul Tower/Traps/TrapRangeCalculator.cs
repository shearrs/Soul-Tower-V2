using Shears;
using Shears.Logging;
using System;
using UnityEngine;

namespace SoulTower.Traps
{
    [RequireComponent(typeof(Trap))]
    public class TrapRangeCalculator : MonoBehaviour
    {
        public event Action<RaycastHit> RangeCalculated;

        private void Awake()
        {
            CoroutineUtil.DoDeferred(() =>
            {
                if (!Physics.Raycast(transform.position, transform.up, out var hit, 20.0f, LayerMask.GetMask("Tower")))
                {
                    SHLogger.Log("Could not find tower surface!", SHLogLevels.Error);
                    return;
                }

                RangeCalculated?.Invoke(hit);
            });
        }
    }
}
