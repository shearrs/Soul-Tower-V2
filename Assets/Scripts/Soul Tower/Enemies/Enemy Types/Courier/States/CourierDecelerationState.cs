using System.Collections;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class CourierDecelerationState : EnemyState
    {
        private readonly Enemy enemy;
        private readonly Courier courier;
        private readonly float decelerationSpeed;
        private readonly IEnemyAnimation idleWalkBlend;

        public CourierDecelerationState(Enemy enemy, Courier courier, float decelerationSpeed, IEnemyAnimation idleWalkBlend)
        {
            Name = "Courier Deceleration State";

            this.enemy = enemy;
            this.courier = courier;
            this.decelerationSpeed = decelerationSpeed;
            this.idleWalkBlend = idleWalkBlend;
        }

        protected override void OnEnter()
        {
            if (courier.IsDecelerating)
            {
                SetAnimationSpeed(idleWalkBlend.Speed);
                CrossFade(idleWalkBlend, 0.1f);
            }
            else
            {
                float t = enemy.ResolvedMoveSpeed / enemy.BaseMoveSpeed;
                SetAnimationSpeed(Mathf.Lerp(idleWalkBlend.Speed, 1.0f, t));
                CrossFade(idleWalkBlend, 0.1f);
            }

            courier.BeginDecelerating();
        }

        protected override void OnExit()
        {
        }

        protected override void OnUpdate()
        {
            if (enemy.ResolvedMoveSpeed == 0.0f)
            {
                EnterStateOfType<CourierRestState>();

                courier.EndDecelerating();
                return;
            }

            enemy.SetMoveSpeed(enemy.MoveSpeed - decelerationSpeed * Time.deltaTime);

            float t = enemy.MoveSpeed / enemy.BaseMoveSpeed;
            SetAnimationSpeed(Mathf.Lerp(1.0f, idleWalkBlend.Speed, t));
            SetBlend(t);
        }
    }
}
