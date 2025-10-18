using Shears;
using Shears.Logging;
using SoulTower.Towers;
using System.Collections;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class EnemyStairsState : EnemyState
    {
        private const float STAIR_CLIMB_TIME = 1.0f;

        private readonly Enemy enemy;
        private readonly IEnemyAnimation animWalk;
        private readonly EnemyState exitState;

        private Coroutine climbCoroutine;

        public EnemyStairsState(Enemy enemy, IEnemyAnimation animWalk, EnemyState exitState)
        {
            Name = "Stairs State";

            this.enemy = enemy;
            this.animWalk = animWalk;
            this.exitState = exitState;
        }

        protected override void OnEnter()
        {
            var nextRoom = enemy.Tower.GetNextRoom(enemy.CurrentRoom);

            if (nextRoom == null)
            {
                Log("Enemy tried to climb stairs, but there was no next room!", SHLogLevels.Error, context: enemy);
                return;
            }
            else if (!enemy.CurrentRoom.HasExitDoor)
            {
                Log("Enemy tried to climb stairs, but there was no exit door in their current room!", SHLogLevels.Error, context: enemy);
                return;
            }
            else if (!nextRoom.HasEntryDoor)
            {
                Log("Enemy tried to climb stairs, but there was no entry door in the next room!", SHLogLevels.Error, context: enemy);
                return;
            }

            climbCoroutine = CoroutineRunner.Start(IETravelThroughStairs(enemy.CurrentRoom.ExitDoor, nextRoom, nextRoom.EntryDoor));
        }

        protected override void OnExit()
        {
            if (climbCoroutine != null)
            {
                CoroutineRunner.Stop(climbCoroutine);
                climbCoroutine = null;
            }
        }

        protected override void OnUpdate()
        {
        }

        private IEnumerator IETravelThroughStairs(Doorway exitDoor, Room nextRoom, Doorway entryDoor)
        {
            SetAnimationSpeed(animWalk.Speed);
            CrossFade(animWalk, 0.1f);

            yield return IEMoveTowards(exitDoor.EntrancePosition);
            yield return IEMoveTowards(exitDoor.StairsPosition);
            yield return IEMoveTowards(exitDoor.ExitPosition);

            yield return CoroutineUtil.WaitForSeconds(STAIR_CLIMB_TIME);

            enemy.CurrentRoom = nextRoom;
            enemy.transform.position = entryDoor.EntrancePosition;

            yield return IEMoveTowards(entryDoor.StairsPosition);
            yield return IEMoveTowards(entryDoor.ExitPosition);

            EnterState(exitState);

            climbCoroutine = null;
        }

        private IEnumerator IEMoveTowards(Vector3 targetPosition)
        {
            while (enemy.transform.position != targetPosition)
            {
                StandardMoveAndRotate(targetPosition, enemy.BaseMoveSpeed);

                yield return null;
            }
        }
    }
}
