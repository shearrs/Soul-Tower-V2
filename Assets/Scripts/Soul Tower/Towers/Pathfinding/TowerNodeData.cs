using Shears.Pathfinding;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Towers
{
    public abstract class TowerNodeData : PathNodeData
    {
        private int entities = 0;

        public int Entities => entities;

        public void RegisterEntity() => entities++;
        public void DeregisterEntity() => entities--;
    }
}
