using Shears.Pathfinding;
using UnityEngine;

namespace SoulTower.Towers
{
    [System.Serializable]
    public class WallNodeData : PathNodeData
    {
        public override Color EditorColor => Color.red;
    }
}
