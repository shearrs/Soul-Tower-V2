using System;
using UnityEngine;

namespace SoulTower.Towers
{
    [Flags]
    public enum TileType 
    { 
        Floor = 1,
        Wall = 2,
        Ceiling = 4
    };
}
