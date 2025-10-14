using Shears.Pathfinding;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Towers
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private PathGrid grid;
        [SerializeField] private PathNode doorNode;

        public Vector3 DoorPosition => doorNode.WorldPosition;

        private void OnValidate()
        {
            if (grid == null)
                return;

            doorNode = grid.GetNodeWithData<DoorNodeData>();
        }

        private void OnDrawGizmosSelected()
        {
            if (doorNode == null)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(doorNode.WorldPosition, Vector3.one);
        }
    }
}
