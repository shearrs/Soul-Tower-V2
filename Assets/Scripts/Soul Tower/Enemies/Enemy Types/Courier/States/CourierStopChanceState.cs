using Shears;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class CourierStopChanceState : EnemyState
    {
        private const float STOP_CHANCE_RATE = 1.0f;

        private readonly Timer stopChanceTimer = new(STOP_CHANCE_RATE);
        private readonly Timer delayTimer;
        private readonly float tiredChance;

        public CourierStopChanceState(float tiredChance, float stopChanceDelay)
        {
            Name = "Courier Stop Chance State";

            this.tiredChance = tiredChance;
            delayTimer = new(stopChanceDelay);
        }

        protected override void OnEnter()
        {
            delayTimer.Start();

            delayTimer.Completed += StartRolling;
            stopChanceTimer.Completed += RollForStop;
        }

        protected override void OnExit()
        {
            delayTimer.Stop();
            stopChanceTimer.Stop();

            delayTimer.Completed -= StartRolling;
            stopChanceTimer.Completed -= RollForStop;
        }

        protected override void OnUpdate()
        {
        }

        private void StartRolling()
        {
            stopChanceTimer.Start();
        }

        private void RollForStop()
        {
            float roll = Random.Range(0.0f, 1.0f);

            if (tiredChance > roll)
            {
                EnterStateOfType<CourierDecelerationState>();
                return;
            }

            stopChanceTimer.Start();
        }
    }
}
