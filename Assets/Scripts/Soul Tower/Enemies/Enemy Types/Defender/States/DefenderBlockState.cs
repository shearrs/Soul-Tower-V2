using Shears;
using UnityEngine;

namespace SoulTower.Enemies
{
    // TODO: if our shield was hit while in this state, reset the timer
    public class DefenderBlockState : EnemyState
    {
        private const float BLOCK_DELAY = 1.0f;

        private readonly Timer blockTimer = new(BLOCK_DELAY);

        public DefenderBlockState()
        {
            Name = "Defender Block State";
        }

        protected override void OnEnter()
        {
            blockTimer.Start();
            blockTimer.Completed += ReturnToNavigation;
        }

        protected override void OnExit()
        {
            blockTimer.Stop();
            blockTimer.Completed -= ReturnToNavigation;
        }

        protected override void OnUpdate()
        {
        }

        private void ReturnToNavigation()
        {
            EnterStateOfType<EnemyFollowPathState>();
        }
    }
}
