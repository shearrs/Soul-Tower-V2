using UnityEngine;

namespace SoulTower.Enemies
{
    public class CourierAccelerationState : EnemyState
    {
        private readonly Enemy enemy;
        private readonly float accelerationSpeed;

        public CourierAccelerationState(Enemy enemy, float accelerationSpeed)
        {
            Name = "Courier Acceleration State";

            this.enemy = enemy;
            this.accelerationSpeed = accelerationSpeed;
        }

        protected override void OnEnter()
        {
            Log("enter acceleration");
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
        }
    }
}
