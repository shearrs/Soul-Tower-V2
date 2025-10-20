using Shears;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class DefenderSwapShieldState : EnemyState
    {
        private readonly static Range<float> SWAP_SHIELD_RANGE = new(5f, 15f);
        private readonly static float SWAP_STOP_TIME = 1.0f;

        private readonly Timer swapTimer = new();
        private readonly Timer swapStopTimer = new(SWAP_STOP_TIME);
        private readonly Defender defender;
        private readonly DefenderShield shield;

        public DefenderSwapShieldState(Defender defender, DefenderShield shield)
        {
            Name = "Defender Swap Shield State";
            this.defender = defender;
            this.shield = shield;
        }

        protected override void OnEnter()
        {
            swapTimer.Start(SWAP_SHIELD_RANGE.Random());
            swapTimer.Completed += SwapShieldDirection;

            shield.HitReceived += OnShieldHit;
            swapStopTimer.Completed += OnSwapStopTimerCompleted;
        }

        protected override void OnExit()
        {
            swapTimer.Stop();
            swapTimer.Completed -= SwapShieldDirection;

            shield.HitReceived -= OnShieldHit;

            swapStopTimer.Stop();
            swapStopTimer.Completed -= OnSwapStopTimerCompleted;

            defender.IsSwappingShield = false;
        }

        protected override void OnUpdate()
        {
        }

        private void SwapShieldDirection()
        {
            shield.RandomizeDirection();
            swapTimer.Start(SWAP_SHIELD_RANGE.Random());

            swapStopTimer.Start();
            defender.IsSwappingShield = true;
            EnterStateOfType<EnemyWaitState>();
        }

        private void OnShieldHit()
        {
            EnterStateOfType<DefenderBlockState>();
        }

        private void OnSwapStopTimerCompleted()
        {
            defender.IsSwappingShield = false;
            EnterStateOfType<EnemyFollowPathState>();
        }
    }
}
