using Shears;
using Shears.HitDetection;
using System;
using UnityEngine;

namespace SoulTower.Traps
{
    [RequireComponent(typeof(Trap))]
    public class FreezeBreeze : ManagedWrapper<Trap>
    {
        [SerializeField] private HitBody3D hitbody;
        private Timer activeTimer = new Timer(2f);

        public event Action<Trap> Activated { add => TypedWrappedValue.Activated += value; remove => TypedWrappedValue.Activated -= value; }
        private void OnEnable()
        {
            Activated += StartFrost;
            activeTimer.Completed += EndFrost;
        }
        private void OnDisable()
        {
            Activated -= StartFrost;
            activeTimer.Completed -= EndFrost;
        }

        private void StartFrost(Trap _)
        {
            hitbody.enabled = true;
            activeTimer.Start();
        }
        private void EndFrost()
        {
            hitbody.enabled = false;
        }
    }
}
