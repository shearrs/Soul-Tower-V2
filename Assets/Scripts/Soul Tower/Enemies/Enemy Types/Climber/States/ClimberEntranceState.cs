using Shears;
using Shears.Detection;
using Shears.Logging;
using Shears.Pathfinding;
using SoulTower.Towers;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class ClimberEntranceState : EnemyState
    {
        private const float CLIMB_DETECTION_RANGE = 100.0f;

        private readonly List<PathNode> path = new();
        private readonly List<TowerNodeData> registeredNodes = new();
        private readonly Timer updatePathTimer;
        private readonly Enemy enemy;
        private readonly Climber climber;
        private readonly EnemyPathfinder pathfinder;
        private readonly SpeedAnimation animWalk;
        private readonly AreaDetector3D climbDetector;

        private Vector3 targetPosition;

        public ClimberEntranceState(Enemy enemy, Climber climber, EnemyPathfinder pathfinder, AreaDetector3D climbDetector, SpeedAnimation animWalk)
        {
            Name = "Climber Entrance State";

            this.enemy = enemy;
            this.climber = climber;
            this.pathfinder = pathfinder;
            this.climbDetector = climbDetector;
            this.animWalk = animWalk;
            updatePathTimer = new(enemy.PathUpdateRate);
        }

        ~ClimberEntranceState()
        {
            foreach (var nodeData in registeredNodes)
                nodeData?.DeregisterEntity(enemy);
        }

        protected override void OnEnter()
        {
            var entryRoom = enemy.Tower.GetEntryRoom();
            var entryRoomGrid = entryRoom.Grid;
            var entranceGrid = enemy.Tower.EntranceGrid;

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

            if (climbDetector.Detect())
            { 
                if (climbDetector.TryGetDetection(out WallOpening opening, true))
                {
                    climber.TargetOpening = opening;

                    EnterStateOfType<ClimberPrepareState>();
                    return;
                }
            }

            if (enemy.transform.position == targetPosition)
            {
                enemy.CurrentRoom = enemy.Tower.GetEntryRoom();
                EnterStateOfType<EnemyNavigationState>();
            }
        }
    }
}
