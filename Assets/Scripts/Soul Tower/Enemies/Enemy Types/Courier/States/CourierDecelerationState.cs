using System.Collections;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class CourierDecelerationState : EnemyState
    {
        private readonly Enemy enemy;
        private readonly Courier courier;
        private readonly float decelerationSpeed;

        public CourierDecelerationState(Enemy enemy, Courier courier, float decelerationSpeed)
        {
            Name = "Courier Deceleration State";

            this.enemy = enemy;
            this.courier = courier;
            this.decelerationSpeed = decelerationSpeed;
        }

        protected override void OnEnter()
        {
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
        }
    }
}
