using System;
using UnityEngine;

namespace SoulTower.Enemies
{

    [Flags]
    public enum EnemySpawnFlags
    {
        Left = 1 << 0,
        Right = 1 << 1,
        SideWithOpening = 1 << 2
    }
}
