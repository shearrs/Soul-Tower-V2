using Shears.Logging;
using SoulTower.Towers;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class VillagerStairsState : VillagerState
    {
        private readonly Tower tower;
        private readonly Enemy enemy;
        private readonly EnemyPathfinder pathfinder;

        public VillagerStairsState(Tower tower, Enemy enemy, EnemyPathfinder pathfinder)
        {
            Name = "Stairs State";

            this.tower = tower;
            this.enemy = enemy;
            this.pathfinder = pathfinder;
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

            EnterStateOfType<VillagerNavigationState>();
        }

        protected override void OnExit()
        {
        }

        protected override void OnUpdate()
        {
        }
    }
}
