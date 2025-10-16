using Shears.Detection;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class EnemyCatalystState : EnemyState
    {
        private readonly AreaDetector3D frontDetector;
        private readonly SpeedAnimation catalystAnim;

        public EnemyCatalystState(AreaDetector3D frontDetector, SpeedAnimation catalystAnim)
        {
            Name = "Catalyst State";

            this.frontDetector = frontDetector;
            this.catalystAnim = catalystAnim;
        }

        protected override void OnEnter()
        {
            SetAnimationSpeed(catalystAnim.Speed);
            CrossFade(catalystAnim, 0.1f);
        }

        protected override void OnExit()
        {
        }

        protected override void OnUpdate()
        {
            //if (!frontDetector.Detect())
            //{
            //    EnterStateOfType<VillagerNavigationState>();
            //    return;
            //}

            //if (!frontDetector.TryGetDetection(out Catalyst _, true))
            //{
            //    EnterStateOfType<VillagerNavigationState>();
            //    return;
            //}

            // attack catalyst
        }
    }
}
