using Shears;
using Shears.HitDetection;
using System;
using UnityEngine;

namespace SoulTower.Traps
{
    [RequireComponent(typeof(Trap))]
    public class FireballTrap : ManagedWrapper<Trap>
    {
        [SerializeField] private FireballProjectile fireballPrefab;
        [SerializeField] private Transform firingPoint;

        public event Action<Trap> Activated { add => TypedWrappedValue.Activated += value; remove => TypedWrappedValue.Activated -= value; }

        public void SpawnFireball()
        {
            var fireball = Instantiate(fireballPrefab);
            fireball.transform.SetPositionAndRotation(firingPoint.position, firingPoint.rotation);
        }
    }
}
