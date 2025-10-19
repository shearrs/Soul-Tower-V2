using Shears;
using Shears.Detection;
using Shears.Logging;
using SoulTower.Traps;
using System.Collections;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class BanditDodgeRollState : EnemyState
    {
        private const float DODGE_END_OFFSET = 1.1f;
        private const float DODGE_ROLL_GRAVITY = 3 * 9.81f;
        private const float DODGE_ROLL_ANGLE = Mathf.Deg2Rad * 35;
        private static readonly float RECIP_COS_ANGLE = 1.0f / Mathf.Cos(DODGE_ROLL_ANGLE);
        private static readonly float COS_ANGLE = Mathf.Cos(DODGE_ROLL_ANGLE);
        private static readonly float SIN_ANGLE = Mathf.Sin(DODGE_ROLL_ANGLE);
        private static readonly float TAN_ANGLE = Mathf.Tan(DODGE_ROLL_ANGLE);

        private readonly Enemy enemy;
        private readonly AreaDetector3D frontDetector;
        private readonly IEnemyAnimation dodgeAnim;

        private Vector3 targetPosition;
        private Coroutine dodgeCoroutine;

        /*  Dodge roll:
         *      - if there is a trap ahead of us, set target destination = right side of trap
         *      - if the right side is further than the exit door, set target destination = exit door entrance
         *      - get the actual target node and calculate the trajectory to take to get there
         *      - move towards the target node in trajectory
        */
        public BanditDodgeRollState(Enemy enemy, AreaDetector3D frontDetector, IEnemyAnimation dodgeAnim)
        {
            Name = "Bandit Dodge Roll State";

            this.enemy = enemy;
            this.frontDetector = frontDetector;
            this.dodgeAnim = dodgeAnim;
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

            Vector3 right = threatArea.GetRight();
            Vector3 endPos = enemy.transform.position.With(x: right.x + DODGE_END_OFFSET);

            Vector3 roomEndPosition;

            if (enemy.CurrentRoom.HasCatalyst)
                roomEndPosition = enemy.CurrentRoom.Catalyst.GetEntrancePosition();
            else
                roomEndPosition = enemy.CurrentRoom.ExitDoor.EntrancePosition;

            if (endPos.x > roomEndPosition.x)
                endPos.x = roomEndPosition.x;

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
            Vector3 planarTarget = targetPosition.XZ();
            Vector3 planarPosition = enemy.transform.position.XZ();
            float distance = Vector3.Distance(planarTarget, planarPosition);
            float yOffset = enemy.transform.position.y - targetPosition.y;
            float initialVelocity = RECIP_COS_ANGLE * Mathf.Sqrt((0.5f * DODGE_ROLL_GRAVITY * Mathf.Pow(distance, 2.0f)) / (distance * TAN_ANGLE + yOffset));
            Vector3 velocity = new(initialVelocity * COS_ANGLE, initialVelocity * SIN_ANGLE, 0);

            // to go in any direction
            //float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPostion) * (p.x > transform.position.x ? 1 : -1);
            //Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

            CrossFade(dodgeAnim, 0.1f);

            while (true)
            {
                enemy.transform.position += Time.deltaTime * velocity;

                if (enemy.transform.position.x >= targetPosition.x)
                    break;

                velocity -= (Time.deltaTime * DODGE_ROLL_GRAVITY * Vector3.up);

                yield return null;
            }

            enemy.transform.position = targetPosition;

            EnterStateOfType<BanditNavigationState>();
            dodgeCoroutine = null;
        }
    }
}
