using Shears;
using Shears.Detection;
using SoulTower.Traps;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class EnemyNavigationState : EnemyState
    {
        private const float DETECTION_RATE = 0.3f;

        private readonly Timer detectionTimer = new(DETECTION_RATE);
        private readonly AreaDetector3D bodyDetector;
        private readonly AreaDetector3D frontDetector;
        private readonly EnemyState blockedState;
        private readonly EnemyState unblockedState;

        public EnemyNavigationState(AreaDetector3D frontDetector, AreaDetector3D bodyDetector, EnemyState blockedState, EnemyState unblockedState)
        {
            Name = "Navigation State";

            this.frontDetector = frontDetector;
            this.bodyDetector = bodyDetector;
            this.blockedState = blockedState;
            this.unblockedState = unblockedState;
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
            detectionTimer.Start();

            frontDetector.Detect();
            bodyDetector.Detect();
            bodyDetector.TryGetDetection(out TrapThreatArea threatAreaBody, true);
            frontDetector.TryGetDetection(out TrapThreatArea threatAreaFront, true);

            bool isBlockingThreat =
                threatAreaFront != null && threatAreaFront != threatAreaBody
                && threatAreaFront.IsActive && threatAreaFront.IsBlocking;

            if (IsInState(blockedState) && !isBlockingThreat)
                EnterState(unblockedState);
            else if (IsInState(unblockedState) && isBlockingThreat)
                EnterState(blockedState);
        }
    }
}
