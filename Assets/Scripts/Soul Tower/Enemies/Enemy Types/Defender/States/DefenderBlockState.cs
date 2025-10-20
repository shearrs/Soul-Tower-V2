using Shears;
using UnityEngine;

namespace SoulTower.Enemies
{
    // TODO: if our shield was hit while in this state, reset the timer
    public class DefenderBlockState : EnemyState
    {
        private const float BLOCK_DELAY = 1.0f;

        private readonly Timer blockTimer = new(BLOCK_DELAY);
        private readonly IEnemyAnimation stopAnim;

        public DefenderBlockState(IEnemyAnimation stopAnim)
        {
            Name = "Defender Block State";
            this.stopAnim = stopAnim;
        }

        protected override void OnEnter()
        {
            blockTimer.Start();
            blockTimer.Completed += ReturnToNavigation;

            SetAnimationSpeed(stopAnim.Speed);
            CrossFade(stopAnim, 0.1f);
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
