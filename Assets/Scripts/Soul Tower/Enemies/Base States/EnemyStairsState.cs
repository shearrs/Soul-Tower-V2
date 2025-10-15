using Shears.Logging;
using SoulTower.Towers;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class EnemyStairsState : EnemyState
    {
        private readonly Tower tower;
        private readonly Enemy enemy;
        private readonly EnemyPathfinder pathfinder;
        private readonly EnemyState exitState;

        public EnemyStairsState(Tower tower, Enemy enemy, EnemyPathfinder pathfinder, EnemyState exitState)
        {
            Name = "Stairs State";

            this.tower = tower;
            this.enemy = enemy;
            this.pathfinder = pathfinder;
            this.exitState = exitState;
        }

        protected override void OnEnter()
        {
            enemy.CurrentRoom = tower.GetNextRoom(enemy.CurrentRoom);

            if (enemy.CurrentRoom != null)
            {
                enemy.transform.position = enemy.CurrentRoom.EntryDoorPosition + enemy.HeightOffset;
                pathfinder.Grid = enemy.CurrentRoom.Grid;
            }
            else
                Log("Enemy tried to climb stairs, but there was no next room!", SHLogLevels.Error, context: enemy);

            EnterState(exitState);
        }

        protected override void OnExit()
        {
        }

        protected override void OnUpdate()
        {
        }
    }
}
