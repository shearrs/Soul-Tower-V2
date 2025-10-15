using Shears;
using Shears.HitDetection;
using System;
using UnityEngine;

namespace SoulTower.Traps
{
    [RequireComponent(typeof(Trap))]
    public class DartTrap : ManagedWrapper<Trap>
    {
        [SerializeField] private DartProjectile dartPrefab;
        [SerializeField] private Transform firingPoint;

        public event Action<Trap> Activated { add => TypedWrappedValue.Activated += value; remove => TypedWrappedValue.Activated -= value; }

        public void SpawnDart()
        {
            var dart = Instantiate(dartPrefab);
            dart.transform.SetPositionAndRotation(firingPoint.position, firingPoint.rotation);
        }
    }
}
