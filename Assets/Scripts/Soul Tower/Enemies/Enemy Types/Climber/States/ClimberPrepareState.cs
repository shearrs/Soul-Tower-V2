using Shears;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class ClimberPrepareState : EnemyState
    {
        private const float PREPARE_TIME = 1.0f;

        private readonly Timer prepareTimer = new(PREPARE_TIME);
        private readonly IEnemyAnimation animPrepare;

        public ClimberPrepareState(IEnemyAnimation animPrepare)
        {
            Name = "Climber Prepare State";

            this.animPrepare = animPrepare;
        }

        protected override void OnEnter()
        {
            prepareTimer.Start();
            prepareTimer.Completed += OnPrepareTimerCompleted;

            CrossFade(animPrepare, 0.1f);
        }

        protected override void OnExit()
        {
            prepareTimer.Stop();
            prepareTimer.Completed -= OnPrepareTimerCompleted;
        }

        protected override void OnUpdate()
        {
        }

        private void OnPrepareTimerCompleted()
        {
            EnterStateOfType<ClimberClimbState>();
        }
    }
}
