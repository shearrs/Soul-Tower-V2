using System;
using UnityEngine;

namespace SoulTower.Traps
{
    [Flags]
    public enum TrapPlacementType 
    { 
        Floor = 1,
        Wall = 2,
        Ceiling = 4
    };
}
