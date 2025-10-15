using Shears;
using Shears.Detection;
using SoulTower.Traps;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class VillagerNavigationState : EnemyState
    {
        private const float DETECTION_RATE = 0.5f;

        private readonly AreaDetector3D frontDetector;
        private readonly AreaDetector3D bodyDetector;

        private readonly Timer detectionTimer = new(DETECTION_RATE);

        public VillagerNavigationState(AreaDetector3D frontDetector, AreaDetector3D bodyDetector)
        {
            Name = "Navigation State";

            this.frontDetector = frontDetector;
            this.bodyDetector = bodyDetector;
        }

        protected override void OnEnter()
        {
            detectionTimer.Start();
            detectionTimer.Completed += DetectThreats;

            DetectThreats();
        }

        protected override void OnExit()
        {
            detectionTimer.Stop();
            detectionTimer.Completed -= DetectThreats;
        }

        protected override void OnUpdate()
        {
        }

        private void DetectThreats()
        {
            bool threat = DetectThreats(frontDetector, bodyDetector);

            if (threat && !IsInStateOfType<EnemyWaitState>())
                EnterStateOfType<EnemyWaitState>();
            else if (!threat && !IsInStateOfType<VillagerFollowPathState>())
                EnterStateOfType<VillagerFollowPathState>();
        }
    }
}
