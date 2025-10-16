using Shears;
using Shears.Logging;
using Shears.Pathfinding;
using SoulTower.Towers;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class EnemyEntranceState : EnemyState
    {
        private readonly List<PathNode> path = new();
        private readonly List<TowerNodeData> registeredNodes = new();
        private readonly Timer updatePathTimer;
        private readonly Enemy enemy;
        private readonly EnemyPathfinder pathfinder;
        private readonly SpeedAnimation animWalk;
        private readonly EnemyState firstState;

        private Vector3 targetPosition;

        public EnemyEntranceState(Enemy enemy, EnemyPathfinder pathfinder, SpeedAnimation animWalk, EnemyState firstState)
        {
            Name = "Entrance State";

            this.enemy = enemy;
            this.pathfinder = pathfinder;
            this.animWalk = animWalk;
            this.firstState = firstState;
            updatePathTimer = new(enemy.PathUpdateRate);
        }

        ~EnemyEntranceState()
        {
            foreach (var nodeData in registeredNodes)
                nodeData?.DeregisterEntity(enemy);
        }

        protected override void OnEnter()
        {
            var entryRoom = enemy.Tower.GetEntryRoom();
            var entryRoomGrid = entryRoom.Grid;
            var entranceGrid = enemy.Tower.LeftEntranceGrid;

            // set our grid to the entrance
            pathfinder.Grid = entranceGrid;

            // set our destination to the main door
            int localZ = Mathf.RoundToInt(enemy.transform.position.z - entryRoomGrid.transform.position.z);
            var entrancePosition = entryRoom.Grid.GetNode(0, 1, localZ).WorldPosition;
            targetPosition = enemy.GetNodePosition(entranceGrid.GetNodeForPosition(entrancePosition).WorldPosition);

            // update our path on an interval
            updatePathTimer.Start();
            updatePathTimer.Completed += UpdatePath;
            UpdatePath();

            // animate walking
            SetAnimationSpeed(animWalk.Speed);
            CrossFade(animWalk, 0.1f);
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
            if (enemy.Tower == null)
            {
                Log("Enemy has no registered tower!", SHLogLevels.Error, context: enemy);
                return;
            }

            foreach (var nodeData in registeredNodes)
                nodeData?.DeregisterEntity(enemy);

            registeredNodes.Clear();
            pathfinder.GetPath(enemy.transform.position, targetPosition, path);

            foreach (var node in path)
            {
                if (node.TryGetData(out TowerNodeData nodeData))
                {
                    nodeData.RegisterEntity(enemy);
                    registeredNodes.Add(nodeData);
                }
            }

            updatePathTimer.Start();
        }

        private void Move()
        {
            StandardPathFollow(path);

            if (enemy.transform.position == targetPosition)
            {
                enemy.CurrentRoom = enemy.Tower.GetEntryRoom();
                EnterState(firstState);
            }
        }
    }
}
