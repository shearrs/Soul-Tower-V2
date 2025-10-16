using UnityEngine;

namespace SoulTower.Enemies
{
    public class CourierAccelerationState : EnemyState
    {
        private readonly Enemy enemy;
        private readonly float accelerationSpeed;
        private readonly SpeedAnimation walkAnim;

        public CourierAccelerationState(Enemy enemy, float accelerationSpeed, SpeedAnimation walkAnim)
        {
            Name = "Courier Acceleration State";

            this.enemy = enemy;
            this.accelerationSpeed = accelerationSpeed;
            this.walkAnim = walkAnim;
        }

        protected override void OnEnter()
        {
        }

        protected override void OnExit()
        {
        }

        protected override void OnUpdate()
        {
            if (enemy.MoveSpeed >= enemy.BaseMoveSpeed)
            {
                EnterStateOfType<CourierStopChanceState>();
                return;
            }

            enemy.SetMoveSpeed(enemy.MoveSpeed + accelerationSpeed * Time.deltaTime);

            float t = enemy.MoveSpeed / enemy.BaseMoveSpeed;
            SetAnimationSpeed(walkAnim.Speed * t);
        }
    }
}
