using Shears.Detection;
using SoulTower.Towers;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class VillagerCatalystState : VillagerState
    {
        private readonly AreaDetector3D frontDetector;

        public VillagerCatalystState(AreaDetector3D frontDetector)
        {
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
