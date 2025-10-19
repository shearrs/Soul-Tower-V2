using Shears;
using Shears.Detection;
using Shears.Logging;
using SoulTower.Traps;
using System.Collections;
using UnityEngine;

namespace SoulTower.Enemies
{
    /* Feint:
    *      - if there is a trap ahead of us, calculate the left position
    *      - set target destination to the left position's node
    *      - move towards the target node
    *      - once reached, wait 1-2 seconds and choose a juke option(backstep, rush, dodgeroll)
    *      - increment feint count/start feint cooldown
    */
    public class BanditFeintState : EnemyState
    {
        private const float FEINT_OFFSET = 0.8f;
        private static readonly Range<float> FEINT_DELAY_RANGE = new(0.5f, 1.0f);

        private readonly Timer feintDelayTimer = new();
        private readonly Enemy enemy;
        private readonly Bandit bandit;
        private readonly AreaDetector3D frontDetector;
        private readonly AreaDetector3D bodyDetector;
        private readonly IEnemyAnimation walkAnim;
        private readonly IEnemyAnimation idleAnim;

        private TrapThreatArea threatArea;
        private Vector3 targetPosition;
        private Coroutine feintCoroutine;

        public BanditFeintState(Enemy enemy, Bandit bandit, AreaDetector3D frontDetector, AreaDetector3D bodyDetector, IEnemyAnimation walkAnim, IEnemyAnimation idleAnim)
        {
            Name = "Bandit Feint State";

            this.enemy = enemy;
            this.bandit = bandit;
            this.frontDetector = frontDetector;
            this.bodyDetector = bodyDetector;
            this.walkAnim = walkAnim;
            this.idleAnim = idleAnim;

            feintDelayTimer.Completed += PerformJuke;
        }

        protected override void OnEnter()
        {
            bandit.IncrementFeintCount();

            int dontFeintRoll = Random.Range(0, 10);

            if (dontFeintRoll == 0)
            {
                bandit.BeginFeintCooldown();
                EnterStateOfType<BanditNavigationState>();

                return;
            }

            if (bodyDetector.Detect())
            {
                if (bodyDetector.TryGetDetection(out TrapThreatArea threat) && threat.IsPrimed)
                {
                    PerformJuke();
                    return;
                }
            }

            if (!frontDetector.Detect())
            {
                Log("Could not detect trap!", SHLogLevels.Verbose);
                EnterStateOfType<BanditNavigationState>();

                return;
            }
            else if (!frontDetector.TryGetDetection(out threatArea) || !threatArea.IsPrimed)
            {
                Log("Could not detect trap threat area!", SHLogLevels.Verbose);
                EnterStateOfType<BanditNavigationState>();

                return;
            }

            Vector3 left = threatArea.GetLeft();
            Vector3 pos = enemy.transform.position.With(x: left.x - FEINT_OFFSET);

            targetPosition = pos;

            feintCoroutine = CoroutineRunner.Start(IEMoveToPosition());
        }

        protected override void OnExit()
        {
            feintDelayTimer.Stop();

            if (feintCoroutine != null)
            {
                CoroutineRunner.Stop(feintCoroutine);
                feintCoroutine = null;
            }
        }

        protected override void OnUpdate()
        {

        }

        private IEnumerator IEMoveToPosition()
        {
            while (true)
            {
                if (!bodyDetector.Detect() && !frontDetector.Detect())
                {
                    Log("Feint lost track of its trap.", SHLogLevels.Verbose);
                    EnterStateOfType<BanditNavigationState>();
                    yield break;
                }

                if (enemy.transform.position != targetPosition)
                {
                    CrossFade(walkAnim, 0.1f);
                    StandardMove(targetPosition);
                    StandardRotate(Quaternion.LookRotation(Vector3.right));
                }
                else
                {
                    CrossFade(idleAnim, 0.1f);
                    feintDelayTimer.Start(FEINT_DELAY_RANGE.Random());
                    break;
                }

                yield return null;
            }

            feintCoroutine = null;
        }

        private void PerformJuke()
        {
            int juke = Random.Range(0, 100);

            EnterStateOfType<BanditDodgeRollState>();
            return;

            switch (juke)
            {
                case int n when n < 20:
                    Log("Bandit chose dodge roll.", SHLogLevels.Verbose);
                    EnterStateOfType<BanditDodgeRollState>();
                    break;
                case int n when n < 50:
                    Log("Bandit chose rush.", SHLogLevels.Verbose);
                    EnterStateOfType<BanditRushState>();
                    break;
                default:
                    Log("Bandit chose backstep.", SHLogLevels.Verbose);
                    EnterStateOfType<BanditBackstepState>();
                    break;
            }
        }
    }
}
