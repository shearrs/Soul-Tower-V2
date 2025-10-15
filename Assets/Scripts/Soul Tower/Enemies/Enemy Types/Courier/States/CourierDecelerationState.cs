using System.Collections;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class CourierDecelerationState : EnemyState
    {
        private readonly Enemy enemy;
        private readonly float decelerationSpeed;

        public CourierDecelerationState(Enemy enemy, float decelerationSpeed)
        {
            Name = "Courier Deceleration State";

            this.enemy = enemy;
            this.decelerationSpeed = decelerationSpeed;
        }

        protected override void OnEnter()
        {
        }

        protected override void OnExit()
        {
        }

        protected override void OnUpdate()
        {
            if (enemy.MoveSpeed == 0.0f)
            {
                EnterStateOfType<CourierRestState>();
                return;
            }

            enemy.SetMoveSpeed(enemy.MoveSpeed - decelerationSpeed * Time.deltaTime);
        }
    }
}
