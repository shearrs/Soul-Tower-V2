using Shears;
using Shears.Pathfinding;
using SoulTower.Towers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class EnemyMovement : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Enemy enemy;
        [SerializeField] private EnemyPathfinder pathfinder;
        [SerializeField] private Tower tower;
        [SerializeField, ReadOnly] private Room currentRoom;

        [Header("Settings")]
        [SerializeField] private Range<float> moveSpeedRange = new(1.0f, 2.0f);
        [SerializeField] private float moveSpeed = 1.0f;

        private readonly List<PathNode> path = new();
        private readonly List<TowerNodeData> registeredNodes = new();

        private void Start()
        {
            moveSpeed = moveSpeedRange.Random();

            currentRoom = tower.GetEntryRoom();
        }

        private void OnDestroy()
        {
            foreach (var nodeData in registeredNodes)
                nodeData.DeregisterEntity(enemy);
        }

        public void Enable()
        {
            StartCoroutine(IEUpdatePath());
            StartCoroutine(IEMove());
        }

        public void Disable()
        {
            StopAllCoroutines();
        }

        private IEnumerator IEUpdatePath()
        {
            while (currentRoom != null)
            {
                foreach (var nodeData in registeredNodes)
                    nodeData.DeregisterEntity(enemy);

                registeredNodes.Clear();
                pathfinder.GetPath(transform.position, currentRoom.DoorPosition, path);

                foreach (var node in path)
                {
                    if (node.TryGetData(out TowerNodeData nodeData))
                    {
                        nodeData.RegisterEntity(enemy);
                        registeredNodes.Add(nodeData);
                    }
                }

                yield return CoroutineUtil.WaitForSeconds(1.0f);
            }
        }

        private IEnumerator IEMove()
        {
            while (true)
            {
                if (transform.position == currentRoom.DoorPosition)
                    currentRoom = tower.GetNextRoom(currentRoom);

                if (currentRoom == null)
                    yield break;

                while (path.Count == 0)
                    yield return null;

                var currentNode = path[0];

                yield return IEMoveToNode(currentNode);

                while (path.Count == 0)
                    yield return null;

                path.RemoveAt(0);
            }
        }

        private IEnumerator IEMoveToNode(PathNode node)
        {
            while (transform.position != node.WorldPosition)
            {
                Vector3 heading = node.WorldPosition - transform.position;
                float magnitude = heading.magnitude;
                Vector3 direction = heading / magnitude;
                float movement = moveSpeed * Time.deltaTime;

                if (magnitude < movement)
                    movement = magnitude;

                transform.position += movement * direction;

                yield return null;
            }
        }
    }
}
