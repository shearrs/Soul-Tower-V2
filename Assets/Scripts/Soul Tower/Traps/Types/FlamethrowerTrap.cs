using Shears;
using Shears.HitDetection;
using System;
using UnityEngine;

namespace SoulTower.Traps
{
    [RequireComponent(typeof(Trap))]
    public class FlamethrowerTrap : ManagedWrapper<Trap>
    {
        [SerializeField] private HitBody3D[] hitbodys;

        private Timer activeTimer = new Timer(2.5f);

        public event Action<Trap> Activated { add => TypedWrappedValue.Activated += value; remove => TypedWrappedValue.Activated -= value; }

        private void OnEnable()
        {
            Activated += StartFlames;
            activeTimer.Completed += EndFlames;
        }
        private void OnDisable()
        {
            Activated -= StartFlames;
            activeTimer.Completed -= EndFlames;
        }

        private void StartFlames(Trap _)
        {
            foreach (var box in hitbodys)
            {
                box.enabled = true;
            }

            activeTimer.Start();
        }
        private void EndFlames()
        {
            foreach (var box in hitbodys)
            {
                box.enabled = false;
            }
        }
    }
}
