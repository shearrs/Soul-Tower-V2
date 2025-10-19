using Shears;
using Shears.Logging;
using SoulTower.Towers;
using System.Collections;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class ClimberClimbState : EnemyState
    {
        private const float CLIMB_SPEED = 1.0f;
        private const float GRAVITY = 9.81f;

        private readonly Enemy enemy;
        private readonly Climber climber;
        private readonly ClimberHook hook;
        private readonly IEnemyAnimation idleAnim;
        private readonly IEnemyAnimation climbAnim;
        private readonly IEnemyAnimation walkAnim;
        private readonly IEnemyAnimation fallAnim;
        private Coroutine climbCoroutine;
        private float velocity = 0.0f;

        public ClimberClimbState(Enemy enemy, Climber climber, ClimberHook hook, IEnemyAnimation idleAnim, IEnemyAnimation climbAnim, IEnemyAnimation walkAnim, IEnemyAnimation fallAnim)
        {
            Name = "Climber Climb State";

            this.enemy = enemy;
            this.climber = climber;
            this.hook = hook;
            this.idleAnim = idleAnim;
            this.climbAnim = climbAnim;
            this.walkAnim = walkAnim;
            this.fallAnim = fallAnim;
        }

        protected override void OnEnter()
        {
            if (climber.TargetOpening == null)
            {
                Log("Target opening is null!", SHLogLevels.Error, context: climber);
                EnterStateOfType<EnemyNavigationState>();

                return;
            }

            CrossFade(idleAnim, 0.1f);

            var opening = climber.TargetOpening;
            var targetPosition = opening.EntrancePosition + (0.55f * Vector3.down);
            targetPosition.z = enemy.transform.position.z;
            Vector3 snapPosition = Vector3.Lerp(targetPosition, opening.FallStartPosition, 0.04f);

            hook.ReachedDestination += Climb;
            hook.Throw(climber.transform.position + Vector3.up, targetPosition, snapPosition);
        }

        protected override void OnExit()
        {
            if (climbCoroutine != null)
            {
                CoroutineRunner.Stop(climbCoroutine);
                climbCoroutine = null;
            }

            velocity = 0.0f;

            if (hook != null)
                hook.ReachedDestination -= Climb;
        }

        protected override void OnUpdate()
        {
        }

        private void Climb()
        {
            climbCoroutine = CoroutineRunner.Start(IEClimb());
        }

        private IEnumerator IEClimb()
        {
            var opening = climber.TargetOpening;
            CrossFade(climbAnim, 0.1f);

            while (climber.transform.position != opening.EntrancePosition)
            {
                StandardMove(opening.EntrancePosition, CLIMB_SPEED);
                yield return null;
            }

            CrossFade(walkAnim, 0.1f);

            while (climber.transform.position != opening.FallStartPosition)
            {
                StandardMove(opening.FallStartPosition);
                yield return null;
            }

            //hook.DestroyAfterUse();
            CrossFade(fallAnim, 0.1f);

            while (climber.transform.position != opening.FallEndPosition)
            {
                velocity += GRAVITY * Time.deltaTime;

                StandardMove(opening.FallEndPosition, velocity);
                yield return null;
            }

            enemy.CurrentRoom = opening.Room;

            if (opening.Room == null)
                SHLogger.Log($"{nameof(WallOpening)} has no room assigned! It needs to be assigned in the Room inspector.", SHLogLevels.Error, context: opening);

            EnterStateOfType<EnemyNavigationState>();
            climbCoroutine = null;
        }
    }
}
