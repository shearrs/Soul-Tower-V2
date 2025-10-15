using Shears.Pathfinding;
using UnityEngine;

namespace SoulTower.Towers
{
    [System.Serializable]
    [NodeDataMenuItem("Door/Exit Door", 3)]
    public class ExitDoorNodeData : TowerNodeData
    {
        public override Color EditorColor => Color.yellow;
    }
}
