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
            AreaDetector3D detector = null;

            if (frontDetector.Detect())
                detector = frontDetector;
            else if (bodyDetector.Detect())
                detector = bodyDetector;

            if (detector != null)
            {
                if (detector.TryGetDetection(out TrapThreatArea threat))
                {
                    if (threat.IsActive)
                        EnterStateOfType<EnemyWaitState>();
                    else if (threat.IsPrimed && bandit.CanFeint)
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
