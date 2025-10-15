using Shears.Logging;
using Shears.Pathfinding;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Towers
{
    public class Room : MonoBehaviour
    {
        [Header("Pathfinding")]
        [SerializeField] private PathGrid grid;
        [SerializeField] private PathNode entryDoorNode;
        [SerializeField] private PathNode exitDoorNode;
        [SerializeField] private PathNode catalystNode;

        [Header("Data")]
        [SerializeField] private Vector3 center;

        public PathGrid Grid => grid;

        public bool HasEntryDoor => entryDoorNode.Data != null;

        public bool HasExitDoor => exitDoorNode.Data != null;

        public bool HasCatalyst => catalystNode.Data != null;

        public Vector3 EntryDoorPosition
        {
            get
            {
                if (HasEntryDoor)
                    return entryDoorNode.WorldPosition;

                SHLogger.Log("Room does not contain an entry door node!", SHLogLevels.Error);

                return Vector3.zero;
            }
        }

        public Vector3 ExitDoorPosition
        {
            get
            {
                if (HasExitDoor)
                    return exitDoorNode.WorldPosition;

                SHLogger.Log("Room does not contain an exit door node!", SHLogLevels.Error);

                return Vector3.zero;
            }
        }

        public Vector3 CatalystPosition
        {
            get
            {
                if (HasCatalyst)
                    return catalystNode.WorldPosition;

                SHLogger.Log("Room does not contain a catalyst node!", SHLogLevels.Error);

                return Vector3.zero;
            }
        }

        public Vector3 Center => transform.TransformPoint(center);

        private void OnValidate()
        {
            if (Application.isPlaying || grid == null)
                return;

            entryDoorNode = grid.GetNodeWithData<EntryDoorNodeData>();
            exitDoorNode = grid.GetNodeWithData<ExitDoorNodeData>();
            catalystNode = grid.GetNodeWithData<CatalystNodeData>();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.TransformPoint(center), 0.15f);

            if (exitDoorNode == null || !HasExitDoor)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(exitDoorNode.WorldPosition, Vector3.one);
        }
    }
}
