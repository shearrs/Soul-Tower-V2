using UnityEngine;

namespace SoulTower.Enemies
{
    public class CourierAccelerationState : EnemyState
    {
        private readonly Enemy enemy;
        private readonly Courier courier;
        private readonly float accelerationSpeed;
        private readonly IEnemyAnimation idleWalkBlend;

        private float velocity = 0.0f;

        public CourierAccelerationState(Enemy enemy, Courier courier, float accelerationSpeed, IEnemyAnimation idleWalkBlend)
        {
            Name = "Courier Acceleration State";

            this.enemy = enemy;
            this.courier = courier;
            this.accelerationSpeed = accelerationSpeed;
            this.idleWalkBlend = idleWalkBlend;
        }

        protected override void OnEnter()
        {
            if (!courier.IsAccelerating)
                velocity = 0.0f;

            courier.BeginAccelerating();
            CrossFade(idleWalkBlend, 0.1f);
        }

        protected override void OnExit()
        {
        }

        protected override void OnUpdate()
        {
            if (enemy.MoveSpeed >= enemy.BaseMoveSpeed)
            {
                courier.EndAccelerating();
                EnterStateOfType<CourierStopChanceState>();
                return;
            }

            velocity += accelerationSpeed * Time.deltaTime;
            enemy.SetMoveSpeed(enemy.MoveSpeed + velocity);

            float t = enemy.MoveSpeed / enemy.BaseMoveSpeed;
            SetAnimationSpeed(Mathf.Lerp(1.0f, idleWalkBlend.Speed, t));
            SetBlend(t);
        }
    }
}
