using Shears;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class CourierRestState : EnemyState
    {
        private readonly float restDuration;
        private readonly Timer timer;

        public CourierRestState(float restDuration)
        {
            Name = "Courier Rest State";

            this.restDuration = restDuration;
            timer = new(restDuration);
        }

        ~CourierRestState()
        {
            timer.Completed -= BeginAcceleration;
        }

        protected override void OnEnter()
        {
            timer.Start();
            timer.Completed += BeginAcceleration;
        }

        protected override void OnExit()
        {
            timer.Stop();
            timer.Completed -= BeginAcceleration;
        }

        protected override void OnUpdate()
        {
        }

        private void BeginAcceleration()
        {
            EnterStateOfType<CourierAccelerationState>();
        }
    }
}
