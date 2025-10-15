using Shears.Pathfinding;
using UnityEngine;

namespace SoulTower.Towers
{
    [System.Serializable]
    [NodeDataMenuItem("Catalyst")]
    public class CatalystNodeData : TowerNodeData
    {
        public override Color EditorColor => Color.limeGreen;
    }
}
