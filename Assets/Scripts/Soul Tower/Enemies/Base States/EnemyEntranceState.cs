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
        private readonly Timer updatePathTimer;
        private readonly Enemy enemy;
        private readonly IEnemyAnimation animWalk;
        private readonly EnemyState firstState;

        private Vector3 targetPosition;

        public EnemyEntranceState(Enemy enemy, IEnemyAnimation animWalk, EnemyState firstState)
        {
            Name = "Entrance State";

            this.enemy = enemy;
            this.animWalk = animWalk;
            this.firstState = firstState;
            updatePathTimer = new(enemy.PathUpdateRate);
        }

        ~EnemyEntranceState()
        {
            DeregisterNodes();
        }

        protected override void OnEnter()
        {
            var entryRoom = enemy.Tower.GetEntryRoom();
            var entryRoomGrid = entryRoom.Grid;
            var entranceGrid = enemy.Tower.LeftEntranceGrid;

            // set our grid to the entrance
            SetGrid(entranceGrid);

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
        }

        protected override void OnUpdate()
        {
            Move();
        }

        private void UpdatePath()
        {
            TargetPathUpdate(targetPosition);
        }

        private void Move()
        {
            StandardPathFollow();

            if (enemy.transform.position == targetPosition)
            {
                enemy.CurrentRoom = enemy.Tower.GetEntryRoom();
                EnterState(firstState);
            }
        }
    }
}
