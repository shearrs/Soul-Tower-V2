using UnityEngine;

namespace SoulTower.Enemies
{
    public class CourierAccelerationState : EnemyState
    {
        private readonly Enemy enemy;
        private readonly float accelerationSpeed;
        private readonly SpeedAnimation idleWalkBlend;

        public CourierAccelerationState(Enemy enemy, float accelerationSpeed, SpeedAnimation idleWalkBlend)
        {
            Name = "Courier Acceleration State";

            this.enemy = enemy;
            this.accelerationSpeed = accelerationSpeed;
            this.idleWalkBlend = idleWalkBlend;
        }

        protected override void OnEnter()
        {
            CrossFade(idleWalkBlend, 0.1f);
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
            SetAnimationSpeed(Mathf.Lerp(1.0f, idleWalkBlend.Speed, t));
            SetBlend(t);
        }
    }
}
