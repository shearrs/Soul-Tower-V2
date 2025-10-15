using Shears.Pathfinding;
using UnityEngine;

namespace SoulTower.Towers
{
    [System.Serializable]
    [NodeDataMenuItem("Tower/Wall", 0)]
    public class WallNodeData : TowerNodeData
    {
        public override Color EditorColor => Color.red;
    }
}
