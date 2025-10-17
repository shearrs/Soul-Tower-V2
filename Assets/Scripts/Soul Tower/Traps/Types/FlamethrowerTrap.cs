using Shears;
using Shears.HitDetection;
using System;
using UnityEngine;

namespace SoulTower.Traps
{
    [RequireComponent(typeof(Trap))]
    public class FlamethrowerTrap : ManagedWrapper<Trap>
    {
        public event Action<Trap> Activated { add => TypedWrappedValue.Activated += value; remove => TypedWrappedValue.Activated -= value; }
    }
}
