using Shears.Detection;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class EnemyCatalystState : EnemyState
    {
        private readonly AreaDetector3D frontDetector;

        public EnemyCatalystState(AreaDetector3D frontDetector)
        {
            Name = "Catalyst State";

            this.frontDetector = frontDetector;
        }

        protected override void OnEnter()
        {
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
