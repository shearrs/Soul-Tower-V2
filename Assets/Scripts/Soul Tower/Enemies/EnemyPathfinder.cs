using Shears;
using Shears.Logging;
using Shears.Pathfinding;
using SoulTower.Towers;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Enemies
{
    [RequireComponent(typeof(Enemy))]
    public class EnemyPathfinder : SHMonoBehaviourLogger
    {
        private const int CARDINAL_COST = 10;
        private const int DIAGONAL_COST = 14;
        private const int ENTITY_COST = 5;

        [SerializeField] private bool drawGizmos = true;
        [SerializeField, ReadOnly] private PathGrid grid;

        private Enemy enemy;

        private readonly Heap<PathNode> openSet = new(32);
        private readonly HashSet<PathNode> closedSet = new();
        private readonly List<PathNode> neighbors = new();
        private readonly List<PathNode> path = new();

        public PathGrid Grid { get => grid; set => grid = value; }

        private void Awake()
        {
            enemy = GetComponent<Enemy>();
        }

        private void OnEnable()
        {
            enemy.RoomChanged += UpdateGrid;
        }

        private void OnDisable()
        {
            enemy.RoomChanged -= UpdateGrid;
        }

        public PathNode GetTargetNode()
        {
            if (path.Count == 0)
                return null;

            return path[0];
        }

        public void GetPath(Vector3 startPos, Vector3 targetPos, List<PathNode> nodes)
        {
            if (grid == null)
            {
                Log("Grid is null!", SHLogLevels.Error);
                return;
            }

            path.Clear();
            UpdatePath(startPos, targetPos);

            nodes.Clear();
            nodes.AddRange(path);
        }

        private void UpdatePath(Vector3 startPos, Vector3 targetPos)
        {
            var startNode = grid.GetNodeForPosition(startPos);
            var targetNode = grid.GetNodeForPosition(targetPos);
            PathNode fallbackTarget = null;

            openSet.Clear();
            closedSet.Clear();
            openSet.Enqueue(startNode);

            while (openSet.Count > 0)
            {
                var currentNode = openSet.Dequeue();
                closedSet.Add(currentNode);

                if (currentNode == targetNode)
                {
                    RetracePath(startNode, targetNode);
                    return;
                }

                grid.GetNeighbors(currentNode, neighbors);

                foreach (var neighbor in neighbors)
                {
                    if (closedSet.Contains(neighbor) || !IsValidNode(neighbor))
                        continue;

                    int totalMovementCost = currentNode.GCost + GetDistance(currentNode, neighbor) + GetWeight(neighbor);
                    bool inOpenSet = openSet.Contains(neighbor);

                    if (totalMovementCost < neighbor.GCost || !inOpenSet)
                    {
                        neighbor.GCost = totalMovementCost;
                        neighbor.HCost = GetDistance(neighbor, targetNode);
                        neighbor.Parent = currentNode;

                        if (fallbackTarget == null || neighbor.HCost < fallbackTarget.HCost)
                            fallbackTarget = neighbor;

                        if (!inOpenSet)
                            openSet.Enqueue(neighbor);
                    }
                }
            }

            if (fallbackTarget != null)
                RetracePath(startNode, fallbackTarget);
            else
            {
                path.Clear();
                path.Add(startNode);
            }
        }

        private void RetracePath(PathNode startNode, PathNode endNode)
        {
            var currentNode = endNode;

            while (currentNode != startNode && currentNode != null)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }

            if (path.Count == 0)
            {
                path.Clear();
                path.Add(startNode);

                return;
            }

            path.Reverse();
        }

        private int GetDistance(PathNode nodeA, PathNode nodeB)
        {
            int xDistance = Mathf.Abs(nodeA.GridPosition.x - nodeB.GridPosition.x);
            int yDistance = Mathf.Abs(nodeA.GridPosition.y - nodeB.GridPosition.y);
            int zDistance = Mathf.Abs(nodeA.GridPosition.z - nodeB.GridPosition.z);

            int min = Mathf.Min(xDistance, yDistance, zDistance);

            return DIAGONAL_COST * min + CARDINAL_COST * (xDistance + yDistance + zDistance - (2 * min));
        }

        private int GetWeight(PathNode node)
        {
            if (!node.TryGetData(out TowerNodeData data))
                return 0;
            else
            {
                int count = data.EntityCount;
                if (data.ContainsEntity(enemy))
                    count--;

                return ENTITY_COST * count;
            }
        }

        private bool IsValidNode(PathNode node)
        {
            if (node.TryGetData(out SurfaceNodeData surfaceData) && surfaceData.Type == SurfaceNodeData.SurfaceType.Floor)
                return true;
            else
                return false;
        }

        private void UpdateGrid(Room room)
        {
            if (room == null)
                grid = null;
            else
                grid = room.Grid;
        }

        private void OnDrawGizmosSelected()
        {
            if (!drawGizmos || grid == null)
                return;

            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(grid.GetNodeForPosition(transform.position).WorldPosition, Vector3.one);

            if (path.Count == 0)
                return;

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(path[^1].WorldPosition, Vector3.one);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, enemy.GetNodePosition(path[0].WorldPosition));

            for (int i = 0; i < path.Count; i++)
            {
                if (i == path.Count - 1)
                    break;

                var node = path[i];

                Gizmos.DrawLine(enemy.GetNodePosition(path[i + 1].WorldPosition), enemy.GetNodePosition(node.WorldPosition));

                int weight = node.FCost + GetWeight(node);
                GizmosUtil.DrawText(enemy.GetNodePosition(path[i].WorldPosition), weight.ToString());
            }
        }
    }
}
