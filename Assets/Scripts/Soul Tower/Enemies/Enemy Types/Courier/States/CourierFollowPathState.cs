using Shears;
using Shears.Pathfinding;
using SoulTower.Towers;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class CourierFollowPathState : EnemyState
    {
        private const float PATH_UPDATE_RATE = 1.0f;
        private const float STOP_CHANCE_RATE = 1.0f;
        private const float STOP_CHANCE = 1.0f; // 0.0f -> 1.0f

        private readonly Enemy enemy;
        private readonly EnemyPathfinder pathfinder;
        private readonly Timer updatePathTimer = new(PATH_UPDATE_RATE);
        private readonly Timer stopChanceTimer = new(STOP_CHANCE_RATE);
        private readonly List<PathNode> path = new();
        private readonly List<TowerNodeData> registeredNodes = new();

        public CourierFollowPathState(Enemy enemy, EnemyPathfinder pathfinder)
        {
            Name = "Courier Follow Path State";

            this.enemy = enemy;
            this.pathfinder = pathfinder;
        }

        ~CourierFollowPathState()
        {
            foreach (var nodeData in registeredNodes)
                nodeData?.DeregisterEntity(enemy);
        }

        protected override void OnEnter()
        {
            updatePathTimer.Start();
            stopChanceTimer.Start();

            updatePathTimer.Completed += UpdatePath;
            stopChanceTimer.Completed += RollForStop;

            UpdatePath();
        }

        protected override void OnExit()
        {
            updatePathTimer.Stop();
            stopChanceTimer.Stop();

            updatePathTimer.Completed -= UpdatePath;
            stopChanceTimer.Completed -= RollForStop;

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

        private void RollForStop()
        {
            float roll = Random.Range(0.0f, 1.0f);

            if (STOP_CHANCE > roll)
            {
                // enter courier stop state
            }
        }
    }
}
