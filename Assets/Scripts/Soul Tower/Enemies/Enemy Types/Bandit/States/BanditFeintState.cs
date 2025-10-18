using Shears;
using Shears.Detection;
using Shears.Logging;
using SoulTower.Traps;
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

        private readonly Enemy enemy;
        private readonly Bandit bandit;
        private readonly AreaDetector3D frontDetector;
        private readonly IEnemyAnimation walkAnim;
        private readonly IEnemyAnimation idleAnim;

        private TrapThreatArea threatArea;
        private Vector3 targetPosition;

        public BanditFeintState(Enemy enemy, Bandit bandit, AreaDetector3D frontDetector, IEnemyAnimation walkAnim, IEnemyAnimation idleAnim)
        {
            Name = "Bandit Feint State";

            this.enemy = enemy;
            this.bandit = bandit;
            this.frontDetector = frontDetector;
            this.walkAnim = walkAnim;
            this.idleAnim = idleAnim;
        }

        protected override void OnEnter()
        {
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
            Vector3 pos = enemy.transform.position.With(x: left.x - FEINT_OFFSET);

            bandit.IncrementFeintCount();
            targetPosition = pos;
        }

        protected override void OnExit()
        {
        }

        protected override void OnUpdate()
        {
            if (!frontDetector.Detect())
            {
                Log("Feint lost track of its trap.");
                EnterStateOfType<BanditNavigationState>();
                return;
            }

            if (enemy.transform.position != targetPosition)
            {
                CrossFade(walkAnim, 0.1f);
                StandardMoveAndRotate(targetPosition);
            }
            else
                CrossFade(idleAnim, 0.1f);
        }
    }
}
