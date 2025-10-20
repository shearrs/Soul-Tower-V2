using Shears;
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

        [Header("Data")]
        [SerializeField, Min(0)] private int cost = 60;
        [SerializeField] private Vector3 center;
        [SerializeField] private bool hasEntryDoor = true;
        [SerializeField] private bool hasExitDoor = true;
        [SerializeField] private bool hasCatalyst = false;
        [SerializeField] private bool hasLeftOpening = false;
        [SerializeField] private bool hasRightOpening = false;

        [Header("Targets")]
        [SerializeField, ShowIf("hasEntryDoor")] private Doorway entryDoor;
        [SerializeField, ShowIf("hasExitDoor")] private Doorway exitDoor;
        [SerializeField, ShowIf("hasCatalyst")] private Catalyst catalyst;
        [SerializeField, ShowIf("hasLeftOpening")] private WallOpening leftOpening;
        [SerializeField, ShowIf("hasRightOpening")] private WallOpening rightOpening;

        private PathNode entryDoorNode;
        private PathNode exitDoorNode;

        public PathGrid Grid => grid;

        public int Cost => cost;

        public bool HasEntryDoor => hasEntryDoor;

        public bool HasExitDoor => hasExitDoor;

        public bool HasCatalyst => hasCatalyst;

        public bool HasLeftOpening => hasLeftOpening;

        public bool HasRightOpening => hasRightOpening;

        public Vector3 EntryDoorPosition
        {
            get
            {
                if (HasEntryDoor)
                {
                    if (!NodeIsValid(entryDoorNode))
                        entryDoorNode = grid.GetNodeForPosition(entryDoor.transform.position);

                    return entryDoorNode.WorldPosition;
                }

                SHLogger.Log("Room does not contain an entry door node!", SHLogLevels.Error);

                return Vector3.zero;
            }
        }

        public Vector3 ExitDoorPosition
        {
            get
            {
                if (HasExitDoor)
                {
                    if (!NodeIsValid(exitDoorNode))
                        exitDoorNode = grid.GetNodeForPosition(exitDoor.transform.position);

                    return exitDoorNode.WorldPosition;
                }

                SHLogger.Log("Room does not contain an exit door node!", SHLogLevels.Error);

                return Vector3.zero;
            }
        }

        public Catalyst Catalyst => catalyst;

        public Doorway EntryDoor => entryDoor;

        public Doorway ExitDoor => exitDoor;

        public Vector3 Center => transform.TransformPoint(center);

        private void Awake()
        {
            if (hasLeftOpening)
                leftOpening.Room = this;

            if (hasRightOpening)
                rightOpening.Room = this;
        }

        public void UpdateGrid()
        {
            grid.UpdateWorldPositions();
        }

        public float GetHeight()
        {
            return grid.NodeSize * grid.GridSize.y;
        }

        private bool NodeIsValid(PathNode node)
        {
            return node != null && node.Data != null;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.TransformPoint(center), 0.15f);
        }
    }
}
