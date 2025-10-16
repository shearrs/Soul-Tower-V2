using Shears;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class CourierRestState : EnemyState
    {
        private readonly SpeedAnimation animIdle;
        private readonly Timer timer;

        public CourierRestState(float restDuration, SpeedAnimation animIdle)
        {
            Name = "Courier Rest State";

            timer = new(restDuration);
            this.animIdle = animIdle;
        }

        ~CourierRestState()
        {
            timer.Completed -= BeginAcceleration;
        }

        protected override void OnEnter()
        {
            timer.Start();
            timer.Completed += BeginAcceleration;

            SetAnimationSpeed(animIdle.Speed);
            CrossFade(animIdle, 0.1f);
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
