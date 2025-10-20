using Shears;
using Shears.HitDetection;
using System;
using UnityEngine;

namespace SoulTower.Traps
{
    [RequireComponent(typeof(Trap))]
    public class OilTrap : ManagedWrapper<Trap>
    {
        [SerializeField] private TrapHitDeliverer deliverer;
        [SerializeField] private HitBody3D hitBody;

        private bool isOnFire;

        public event Action LitOnFire;

        public bool IsOnFire => isOnFire;

        public void LightOnFire()
        {
            isOnFire = true;
            LitOnFire?.Invoke();

            hitBody.CollisionMask = LayerMask.GetMask("Enemy", "Trap Receiver");
            deliverer.AddData(new(1, HitDetection.DamageType.Fire));
        }
    }
}
