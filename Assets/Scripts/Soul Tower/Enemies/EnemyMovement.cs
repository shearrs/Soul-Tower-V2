using Shears;
using Shears.Detection;
using Shears.Logging;
using Shears.Pathfinding;
using SoulTower.Towers;
using SoulTower.Traps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class EnemyMovement : MonoBehaviour
    {
        [Header("Enemy Components")]
        [SerializeField] private Enemy enemy;
        [SerializeField] private EnemyPathfinder pathfinder;
        [SerializeField] private AreaDetector3D futureTrapDetector;
        [SerializeField] private AreaDetector3D bodyTrapDetector;

        [Header("Tower Components")]
        [SerializeField] private Tower tower;
        [SerializeField, ReadOnly] private Room currentRoom;

        [Header("Settings")]
        [SerializeField] private Range<float> moveSpeedRange = new(1.0f, 2.0f);
        [SerializeField] private float moveSpeed = 1.0f;

        private readonly List<PathNode> path = new();
        private readonly List<TowerNodeData> registeredNodes = new();

        private Coroutine updatePathRoutine;

        private void Awake()
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

                Vector3 targetPosition = Vector3.zero;

                if (currentRoom.HasExitDoor)
                    targetPosition = currentRoom.ExitDoorPosition;
                else if (currentRoom.HasCatalyst)
                    targetPosition = currentRoom.CatalystPosition;
                else
                    SHLogger.Log("Room has no exit door or catalyst!", SHLogLevels.Error);

                pathfinder.GetPath(transform.position, targetPosition, path);

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
                if (currentRoom.HasCatalyst && transform.position == currentRoom.CatalystPosition)
                    yield break;

                if (currentRoom.HasExitDoor && transform.position == currentRoom.ExitDoorPosition)
                    currentRoom = tower.GetNextRoom(currentRoom);

                if (currentRoom == null)
                    yield break;

                if (updatePathRoutine != null)
                    StopCoroutine(updatePathRoutine);

                pathfinder.Grid = currentRoom.Grid;
                updatePathRoutine = StartCoroutine(IEUpdatePath());

                if (currentRoom != tower.GetEntryRoom())
                    transform.position = currentRoom.EntryDoorPosition;

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
                // could definitely afford to not do this every frame
                if (futureTrapDetector.Detect())
                {
                    bodyTrapDetector.Detect();

                    if (futureTrapDetector.TryGetDetection(out TrapThreatArea threatArea) && !bodyTrapDetector.TryGetDetection(out TrapThreatArea _))
                    {
                        while (threatArea != null && threatArea.IsActive)
                            yield return null;
                    }
                }

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
