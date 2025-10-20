using Shears;
using Shears.Detection;
using SoulTower.Traps;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class DefenderNavigationState : EnemyState
    {
        private const float DETECTION_RATE = 0.3f;

        private readonly Timer detectionTimer = new(DETECTION_RATE);
        private readonly Defender defender;
        private readonly AreaDetector3D bodyDetector;
        private readonly AreaDetector3D frontDetector;

        public DefenderNavigationState(Defender defender, AreaDetector3D frontDetector, AreaDetector3D bodyDetector)
        {
            Name = "Navigation State";

            this.defender = defender;
            this.frontDetector = frontDetector;
            this.bodyDetector = bodyDetector;
        }

        protected override void OnEnter()
        {
            detectionTimer.Start();
            detectionTimer.Completed += UpdateDetection;

            UpdateDetection();
        }

        protected override void OnExit()
        {
            detectionTimer.Stop();
            detectionTimer.Completed -= UpdateDetection;
        }

        protected override void OnUpdate()
        {

        }

        private void UpdateDetection()
        {
            if (defender.IsSwappingShield)
                return;

            detectionTimer.Start();

            frontDetector.Detect();
            bodyDetector.Detect();
            bodyDetector.TryGetDetection(out TrapThreatArea threatAreaBody, true);
            frontDetector.TryGetDetection(out TrapThreatArea threatAreaFront, true);

            bool isBlockingThreat =
                threatAreaFront != null && threatAreaFront != threatAreaBody
                && threatAreaFront.IsActive && threatAreaFront.IsBlocking;

            if (IsInStateOfType<EnemyWaitState>() && !isBlockingThreat)
                EnterStateOfType<EnemyFollowPathState>();
            else if (IsInStateOfType<EnemyFollowPathState>() && isBlockingThreat)
                EnterStateOfType<EnemyWaitState>();
        }
    }
}
