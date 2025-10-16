using Shears;
using Shears.Logging;
using Shears.Pathfinding;
using SoulTower.Towers;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Enemies
{
    [RequireComponent(typeof(Enemy))]
    public class EnemyPathfinder : MonoBehaviour
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

        private void Start()
        {
            SHLogger.Log("Enemy default grid temporarily set by pathfinder.");
            Grid = enemy.CurrentRoom.Grid;
        }

        public PathNode GetTargetNode()
        {
            if (path.Count == 0)
                return null;

            return path[0];
        }

        public void GetPath(Vector3 startPos, Vector3 targetPos, List<PathNode> nodes)
        {
            path.Clear();
            UpdatePath(startPos, targetPos);

            nodes.Clear();
            nodes.AddRange(path);
        }

        public void UpdatePath(Vector3 startPos, Vector3 targetPos)
        {
            var startNode = grid.GetNodeForPosition(startPos);
            var targetNode = grid.GetNodeForPosition(targetPos);

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

                        if (!inOpenSet)
                            openSet.Enqueue(neighbor);
                    }
                }
            }

            RetracePath(startNode, targetNode);
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
            Gizmos.DrawLine(transform.position, path[0].WorldPosition + enemy.HeightOffset);

            for (int i = 0; i < path.Count; i++)
            {
                if (i == path.Count - 1)
                    break;

                var node = path[i];

                Gizmos.DrawLine(path[i + 1].WorldPosition + enemy.HeightOffset, node.WorldPosition + enemy.HeightOffset);

                int weight = node.FCost + GetWeight(node);
                GizmosUtil.DrawText(path[i].WorldPosition + enemy.HeightOffset, weight.ToString());
            }
        }
    }
}
