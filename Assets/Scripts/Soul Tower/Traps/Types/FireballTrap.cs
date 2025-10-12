using Shears;
using Shears.HitDetection;
using System;
using UnityEngine;

namespace SoulTower.Traps
{
    [RequireComponent(typeof(Trap))]
    public class FireballTrap : ManagedWrapper<Trap>
    {
        //private Trap trap;

        public event Action Activated { add => TypedWrappedValue.Activated += value; remove => TypedWrappedValue.Activated -= value; }

        /*private void Awake()
        {
            trap = GetComponent<Trap>();
        }*/

        private void OnEnable()
        {
            TypedWrappedValue.Activated += OnActivated;
        }

        private void OnDisable()
        {
            TypedWrappedValue.Activated -= OnActivated;
        }

        private void OnActivated()
        {

        }

        private void OnTimerEnd()
        {

        }
    }
}
