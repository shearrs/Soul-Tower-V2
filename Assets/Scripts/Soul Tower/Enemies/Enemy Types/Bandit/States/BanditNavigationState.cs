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

        private void DetectThreats()
        {
            TrapThreatArea threat = null;
            detectionTimer.Start();

            if (frontDetector.Detect() && frontDetector.TryGetDetection(out threat, true) && threat.IsActive && threat.IsBlocking)
            {
                EnterStateOfType<EnemyWaitState>();
                return;
            }
            else if (IsInStateOfType<EnemyWaitState>())
                EnterStateOfType<EnemyFollowPathState>();

            if (threat == null)
            {
                if (!bodyDetector.TryGetDetection(out threat, true))
                    return;
            }

            if (threat.IsPrimed && bandit.CanFeint)
            {
                EnterStateOfType<BanditFeintState>();
                return;
            }
        }
    }
}
