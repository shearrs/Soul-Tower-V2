using Shears;
using Shears.Pathfinding;
using SoulTower.Towers;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class EnemyFollowPathState : EnemyState
    {
        private readonly List<PathNode> path = new();
        private readonly List<TowerNodeData> registeredNodes = new();
        private readonly Timer updatePathTimer;
        private readonly Enemy enemy;
        private readonly EnemyPathfinder pathfinder;
        private readonly SpeedAnimation animWalk;

        public EnemyFollowPathState(Enemy enemy, EnemyPathfinder pathfinder, SpeedAnimation animWalk)
        {
            Name = "Enemy Follow Path State";

            this.enemy = enemy;
            this.pathfinder = pathfinder;
            this.animWalk = animWalk;
            updatePathTimer = new(enemy.PathUpdateRate);
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
            StandardPathUpdate(pathfinder, path, registeredNodes);
        }

        private void Move()
        {
            StandardPathFollow(path);
        }
    }
}
