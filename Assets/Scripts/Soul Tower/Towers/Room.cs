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
        private PathNode catalystNode;

        public PathGrid Grid => grid;

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

        public Vector3 CatalystPosition
        {
            get
            {
                if (HasCatalyst)
                {
                    if (!NodeIsValid(catalystNode))
                        catalystNode = grid.GetNodeForPosition(catalyst.transform.position);

                    return catalystNode.WorldPosition;
                }

                SHLogger.Log("Room does not contain a catalyst node!", SHLogLevels.Error);

                return Vector3.zero;
            }
        }

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
