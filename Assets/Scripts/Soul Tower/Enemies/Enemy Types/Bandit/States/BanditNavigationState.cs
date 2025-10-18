using Shears;
using Shears.Detection;
using SoulTower.Traps;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class BanditNavigationState : EnemyState
    {
        private const float DETECTION_RATE = 0.1f;

        private readonly Bandit bandit;
        private readonly AreaDetector3D frontDetector;
        private readonly AreaDetector3D bodyDetector;
        private readonly Timer detectionTimer = new(DETECTION_RATE);

        private bool bubbleUpTick = false;

        public BanditNavigationState(Bandit bandit, AreaDetector3D frontDetector, AreaDetector3D bodyDetector)
        {
            Name = "Bandit Navigation State";

            this.bandit = bandit;
            this.frontDetector = frontDetector;
            this.bodyDetector = bodyDetector;
        }

        protected override void OnEnter()
        {
            detectionTimer.Start();
            detectionTimer.Completed += DetectThreats;

            bubbleUpTick = true;
        }

        protected override void OnExit()
        {
            detectionTimer.Stop();
            detectionTimer.Completed -= DetectThreats;

            bubbleUpTick = false;
        }

        protected override void OnUpdate()
        {
            if (bubbleUpTick)
            {
                DetectThreats();
                bubbleUpTick = false;
            }
        }

        /* 
         * Backstep:
         *      - if there is a trap ahead of us, calculate a position a certain amount of depth into the trap
         *      - set target destination to the left position's node
         *      - quickly move towards the target node
         *      - once reached
         * 
         * Rush:
         *      - if there is a trap ahead of us, increase move speed until we are out of a trap threat area
         * 
         * Dodge roll:
         *      - if there is a trap ahead of us, set target destination = right side of trap
         *      - if the right side is further than the exit door, set target destination = exit door entrance
         *      - get the actual target node and calculate the trajectory to take to get there
         *      - move towards the target node in trajectory
        */

        // for now lets just always choose feint
        private void DetectThreats()
        {
            if (frontDetector.Detect())
            {
                if (frontDetector.TryGetDetection(out TrapThreatArea threat))
                {
                    if (threat.IsActive)
                        EnterStateOfType<EnemyWaitState>();
                    else if (bandit.CanFeint)
                    {
                        EnterStateOfType<BanditFeintState>();
                        return;
                    }
                }
            }

            detectionTimer.Start();
        }
    }
}
