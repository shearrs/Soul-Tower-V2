using Shears;
using Shears.Detection;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class EnemyNavigationState : EnemyState
    {
        private const float DETECTION_RATE = 0.5f;

        private readonly AreaDetector3D frontDetector;
        private readonly AreaDetector3D bodyDetector;
        private readonly EnemyState threatState;
        private readonly EnemyState noThreatState;
        private readonly Timer detectionTimer = new(DETECTION_RATE);

        private bool bubbleUpTick = false;

        public EnemyNavigationState(AreaDetector3D frontDetector, AreaDetector3D bodyDetector, EnemyState threatState, EnemyState noThreatState)
        {
            Name = "Navigation State";

            this.frontDetector = frontDetector;
            this.bodyDetector = bodyDetector;
            this.threatState = threatState;
            this.noThreatState = noThreatState;
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

        private void DetectThreats()
        {
            bool threat = DetectThreats(frontDetector, bodyDetector);

            if (threat && !IsInState(threatState))
                EnterState(threatState);
            else if (!threat && !IsInState(noThreatState))
                EnterState(noThreatState);
        }
    }
}
