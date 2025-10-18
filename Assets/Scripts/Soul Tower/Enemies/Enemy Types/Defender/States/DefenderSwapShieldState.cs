using Shears;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class DefenderSwapShieldState : EnemyState
    {
        private readonly static Range<float> SWAP_SHIELD_RANGE = new(5f, 15f);

        private readonly Timer swapTimer = new();
        private readonly DefenderShield shield;

        public DefenderSwapShieldState(DefenderShield shield)
        {
            Name = "Defender Swap Shield State";
            this.shield = shield;
        }

        protected override void OnEnter()
        {
            swapTimer.Start(SWAP_SHIELD_RANGE.Random());
            swapTimer.Completed += SwapShieldDirection;

            shield.HitReceived += OnShieldHit;
        }

        protected override void OnExit()
        {
            swapTimer.Stop();
            swapTimer.Completed -= SwapShieldDirection;

            shield.HitReceived -= OnShieldHit;
        }

        protected override void OnUpdate()
        {
        }

        private void SwapShieldDirection()
        {
            shield.RandomizeDirection();
            swapTimer.Start(SWAP_SHIELD_RANGE.Random());
        }

        private void OnShieldHit()
        {
            EnterStateOfType<DefenderBlockState>();
        }
    }
}
