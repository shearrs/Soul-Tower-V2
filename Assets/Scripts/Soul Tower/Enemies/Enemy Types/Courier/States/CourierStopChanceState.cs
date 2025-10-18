using Shears;
using Shears.Logging;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class CourierStopChanceState : EnemyState
    {
        private const float STOP_CHANCE_RATE = 0.5f;

        private readonly Timer stopChanceTimer = new(STOP_CHANCE_RATE);
        private readonly Courier courier;
        private readonly float tiredChance;

        public CourierStopChanceState(Courier courier, float tiredChance)
        {
            Name = "Courier Stop Chance State";

            this.courier = courier;
            this.tiredChance = tiredChance;
        }

        protected override void OnEnter()
        {
            if (courier.IsAccelerating)
            {
                EnterStateOfType<CourierAccelerationState>();
                return;
            }
            else if (courier.IsDecelerating)
            {
                EnterStateOfType<CourierDecelerationState>();
                return;
            }

            if (courier.IsStopOnCooldown())
                courier.AddStopCooldownEvent(BeginStopRolls);
            else
                BeginStopRolls();

            stopChanceTimer.Completed += RollForStop;
        }

        protected override void OnExit()
        {
            courier.RemoveStopCooldownEvent(BeginStopRolls);
            stopChanceTimer.Stop();

            stopChanceTimer.Completed -= RollForStop;
        }

        protected override void OnUpdate()
        {
        }

        private void BeginStopRolls()
        {
            stopChanceTimer.Start();
        }

        private void RollForStop()
        {
            float roll = Random.Range(0.0f, 1.0f);

            Log($"Rolled {roll} with chance of {tiredChance}", SHLogLevels.Verbose);

            if (tiredChance > roll)
            {
                EnterStateOfType<CourierDecelerationState>();
                return;
            }

            stopChanceTimer.Start();
        }
    }
}
