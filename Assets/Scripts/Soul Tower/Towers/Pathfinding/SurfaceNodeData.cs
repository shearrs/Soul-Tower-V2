using Shears;
using Shears.Pathfinding;
using UnityEditor;
using UnityEngine;

namespace SoulTower.Towers
{
    [System.Serializable]
    public class SurfaceNodeData : PathNodeData
    {
        public enum SurfaceType { Floor, Wall, Ceiling };

        [SerializeField] private SurfaceType surfaceType = SurfaceType.Floor;
        [SerializeField] private bool rightWall = true;

        public SurfaceType Type => surfaceType;
        public override Color EditorColor => Color.green;

#if UNITY_EDITOR
        public override void DrawHandles(Vector3 nodePosition, float nodeSize)
        {
            Handles.color = Color.red;

            Vector3 direction;
            Vector3 headDirection;

            switch (surfaceType)
            {
                case SurfaceType.Floor:
                    direction = Vector3.up;
                    headDirection = Vector3.right;
                    break;
                case SurfaceType.Wall:
                    direction = rightWall ? Vector3.left : Vector3.right;
                    headDirection = Vector3.down;
                    break;
                case SurfaceType.Ceiling:
                    direction = Vector3.down;
                    headDirection = Vector3.right;
                    break;
                default:
                    direction = Vector3.zero;
                    headDirection = Vector3.up;
                    break;
            }

            Vector3 offset = nodeSize * direction;
            Vector3 from = nodePosition - (0.5f * offset);
            Vector3 headMiddle = from + ((nodeSize - 0.1f) * direction);
            Vector3 head1End = headMiddle + (0.1f * headDirection);
            Vector3 head2End = headMiddle - (0.1f * headDirection);

            Handles.DrawLine(from, from + offset);

            Handles.color = Color.yellow;
            Handles.DrawLine(from + offset, head1End);
            Handles.DrawLine(from + offset, head2End);
        }
#endif
    }
}
