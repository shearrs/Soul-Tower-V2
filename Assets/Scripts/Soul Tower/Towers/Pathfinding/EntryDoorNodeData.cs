using Shears.Pathfinding;
using UnityEngine;

namespace SoulTower.Towers
{
    [System.Serializable]
    [NodeDataMenuItem("Door/Entry Door", 2)]
    public class EntryDoorNodeData : TowerNodeData
    {
        public override Color EditorColor => Color.cyan;
    }
}
