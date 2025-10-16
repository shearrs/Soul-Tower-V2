using System.Collections;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class CourierDecelerationState : EnemyState
    {
        private readonly Enemy enemy;
        private readonly Courier courier;
        private readonly float decelerationSpeed;
        private readonly SpeedAnimation idleWalkBlend;

        public CourierDecelerationState(Enemy enemy, Courier courier, float decelerationSpeed, SpeedAnimation idleWalkBlend)
        {
            Name = "Courier Deceleration State";

            this.enemy = enemy;
            this.courier = courier;
            this.decelerationSpeed = decelerationSpeed;
            this.idleWalkBlend = idleWalkBlend;
        }

        protected override void OnEnter()
        {
            CrossFade(idleWalkBlend, 0.1f);

            courier.BeginStopping();
        }

        protected override void OnExit()
        {
        }

        protected override void OnUpdate()
        {
            if (enemy.MoveSpeed == 0.0f)
            {
                EnterStateOfType<CourierRestState>();

                courier.EndStopping();
                return;
            }

            enemy.SetMoveSpeed(enemy.MoveSpeed - decelerationSpeed * Time.deltaTime);

            float t = enemy.MoveSpeed / enemy.BaseMoveSpeed;
            SetAnimationSpeed(Mathf.Lerp(idleWalkBlend.Speed, 1.0f, t));
            SetBlend(idleWalkBlend, t);
        }
    }
}
