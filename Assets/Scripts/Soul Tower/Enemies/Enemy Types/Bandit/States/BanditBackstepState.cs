using Shears;
using Shears.Detection;
using Shears.Logging;
using SoulTower.Traps;
using System.Collections;
using UnityEngine;

namespace SoulTower.Enemies
{
    /* 
    * Backstep:
    *      - if there is a trap ahead of us, calculate a position a certain amount of depth into the trap
    *      - set target destination to the left position's node
    *      - quickly move towards the target node
    *      - once reached
    */
    public class BanditBackstepState : EnemyState
    {
        private const float BACKSTEP_DELAY_TIME = 0.5f;
        private const float BACKSTEP_END_OFFSET = 1.1f;
        private const float BACKSTEP_SPEED_MULT = 4.25f;

        private readonly Timer delayTimer = new(BACKSTEP_DELAY_TIME);
        private readonly Enemy enemy;
        private readonly AreaDetector3D frontDetector;
        private readonly IEnemyAnimation walkAnim;

        private Vector3 targetPosition;
        private Coroutine dodgeCoroutine;

        public BanditBackstepState(Enemy enemy, AreaDetector3D frontDetector, IEnemyAnimation walkAnim)
        {
            Name = "Bandit Backstep State";

            this.enemy = enemy;
            this.frontDetector = frontDetector;
            this.walkAnim = walkAnim;
        }

        protected override void OnEnter()
        {
            TrapThreatArea threatArea;

            if (!frontDetector.Detect())
            {
                Log("Could not detect trap!", SHLogLevels.Error);
                return;
            }
            else if (!frontDetector.TryGetDetection(out threatArea))
            {
                Log("Could not detect trap threat area!", SHLogLevels.Error);
                return;
            }

            Vector3 left = threatArea.GetLeft();
            Vector3 endPos = enemy.transform.position.With(x: left.x - BACKSTEP_END_OFFSET);

            SetAnimationSpeed(walkAnim.Speed);
            CrossFade(walkAnim, 0.1f);

            targetPosition = endPos;
            dodgeCoroutine = CoroutineRunner.Start(IEDodge());
        }

        protected override void OnExit()
        {
            if (dodgeCoroutine != null)
            {
                CoroutineRunner.Stop(dodgeCoroutine);
                dodgeCoroutine = null;
            }
        }

        protected override void OnUpdate()
        {
        }

        private IEnumerator IEDodge()
        {
            delayTimer.Restart();

            while (!delayTimer.IsDone)
            {
                StandardPathUpdate();
                StandardPathFollow();

                yield return null;
            }

            while (enemy.transform.position != targetPosition)
            {
                StandardMove(targetPosition, BACKSTEP_SPEED_MULT * enemy.ResolvedMoveSpeed);

                yield return null;
            }

            dodgeCoroutine = null;
            EnterStateOfType<BanditNavigationState>();
        }
    }
}
