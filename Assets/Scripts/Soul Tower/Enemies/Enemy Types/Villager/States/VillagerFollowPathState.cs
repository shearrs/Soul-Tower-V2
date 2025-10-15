using Shears;
using Shears.Logging;
using Shears.Pathfinding;
using SoulTower.Towers;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class VillagerFollowPathState : VillagerState
    {
        private const float PATH_UPDATE_RATE = 1.0f;

        private readonly Enemy enemy;
        private readonly EnemyPathfinder pathfinder;
        private readonly Timer updatePathTimer = new(PATH_UPDATE_RATE);
        private readonly List<PathNode> path = new();
        private readonly List<TowerNodeData> registeredNodes = new();

        private Room CurrentRoom => enemy.CurrentRoom;

        public VillagerFollowPathState(Enemy enemy, EnemyPathfinder pathfinder)
        {
            Name = "Follow Path State";

            this.enemy = enemy;
            this.pathfinder = pathfinder;
        }

        ~VillagerFollowPathState()
        {
            foreach (var nodeData in registeredNodes)
                nodeData?.DeregisterEntity(enemy);
        }

        protected override void OnEnter()
        {
            updatePathTimer.Start();
            updatePathTimer.Completed += UpdatePath;

            UpdatePath();
        }

        protected override void OnExit()
        {
            updatePathTimer.Stop();
            updatePathTimer.Completed -= UpdatePath;

            foreach (var nodeData in registeredNodes)
                nodeData?.DeregisterEntity(enemy);
        }

        protected override void OnUpdate()
        {
            Move();
        }

        private void UpdatePath()
        {
            if (CurrentRoom == null)
                return;

            foreach (var nodeData in registeredNodes)
                nodeData.DeregisterEntity(enemy);

            registeredNodes.Clear();

            Vector3 targetPosition = Vector3.zero;

            if (CurrentRoom.HasExitDoor)
                targetPosition = CurrentRoom.ExitDoorPosition;
            else if (CurrentRoom.HasCatalyst)
                targetPosition = CurrentRoom.CatalystPosition;
            else
            {
                Log("Room has no exit door or catalyst!", SHLogLevels.Error);

                path.Clear();
                return;
            }

            pathfinder.GetPath(enemy.transform.position, targetPosition, path);

            foreach (var node in path)
            {
                if (node.TryGetData(out TowerNodeData nodeData))
                {
                    nodeData.RegisterEntity(enemy);
                    registeredNodes.Add(nodeData);
                }
            }
        }
    
        private void Move()
        {
            if (CurrentRoom == null)
            {
                Log("Current room is null!", SHLogLevels.Error);
                return;
            }

            if (IsAtCatalyst())
            {
                EnterStateOfType<VillagerCatalystState>();
                return;
            }
            else if (IsAtExitDoor())
            {
                EnterStateOfType<VillagerStairsState>();
                return;
            }

            if (path.Count == 0)
            {
                Log($"Enemy {enemy.name} cannot find path!", SHLogLevels.Warning, context: enemy);
                return;
            }

            var node = path[0];
            Vector3 heading = node.WorldPosition - enemy.transform.position;
            float magnitude = heading.magnitude;
            Vector3 direction = heading / magnitude;
            float movement = enemy.MoveSpeed * Time.deltaTime;

            if (magnitude < movement)
                movement = magnitude;

            if (magnitude > 0.001f)
                enemy.transform.position += movement * direction;
            else
            {
                enemy.transform.position = node.WorldPosition;
                path.RemoveAt(0);
            }
        }

        private bool IsAtCatalyst()
        {
            return CurrentRoom != null && CurrentRoom.HasCatalyst && enemy.transform.position == CurrentRoom.CatalystPosition;
        }

        private bool IsAtExitDoor()
        {
            return CurrentRoom != null && CurrentRoom.HasExitDoor && enemy.transform.position == CurrentRoom.ExitDoorPosition;
        }
    }
}
