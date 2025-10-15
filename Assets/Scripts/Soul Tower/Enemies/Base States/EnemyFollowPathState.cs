using Shears;
using Shears.Pathfinding;
using SoulTower.Towers;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class EnemyFollowPathState : EnemyState
    {
        private const float PATH_UPDATE_RATE = 1.0f;

        private readonly Enemy enemy;
        private readonly EnemyPathfinder pathfinder;
        private readonly Timer updatePathTimer = new(PATH_UPDATE_RATE);
        private readonly List<PathNode> path = new();
        private readonly List<TowerNodeData> registeredNodes = new();

        public EnemyFollowPathState(Enemy enemy, EnemyPathfinder pathfinder)
        {
            Name = "Enemy Follow Path State";

            this.enemy = enemy;
            this.pathfinder = pathfinder;
        }

        ~EnemyFollowPathState()
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
            StandardPathUpdate(pathfinder, path, registeredNodes);
        }

        private void Move()
        {
            StandardMove(path);
        }
    }
}
