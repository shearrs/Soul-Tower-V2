using Shears;
using Shears.Detection;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class BanditRushState : EnemyState
    {
        private const float MIN_RUSH_TIME = 1.5f;
        private const float RUSH_SPEED_MULT = 3.0f;

        private readonly Timer minRushTimer = new(MIN_RUSH_TIME);
        private readonly Timer pathUpdateTimer = new();
        private readonly Enemy enemy;
        private readonly AreaDetector3D bodyDetector;
        private readonly IEnemyAnimation walkAnim;

        private float previousBaseSpeed;

        public BanditRushState(Enemy enemy, AreaDetector3D bodyDetector, IEnemyAnimation walkAnim)
        {
            Name = "Bandit Rush State";
            
            this.enemy = enemy;
            this.bodyDetector = bodyDetector;
            this.walkAnim = walkAnim;
        }

        protected override void OnEnter()
        {
            previousBaseSpeed = enemy.BaseMoveSpeed;
            enemy.SetMoveSpeed(enemy.BaseMoveSpeed * RUSH_SPEED_MULT, true);

            minRushTimer.Start();
            pathUpdateTimer.Start(enemy.PathUpdateRate);
            pathUpdateTimer.Completed += UpdatePath;

            SetAnimationSpeed(walkAnim.Speed);
            CrossFade(walkAnim, 0.1f);

            StandardPathUpdate();
        }

        protected override void OnExit()
        {
            minRushTimer.Stop();
            pathUpdateTimer.Stop();
            pathUpdateTimer.Completed -= UpdatePath;

            enemy.SetMoveSpeed(previousBaseSpeed);
        }

        protected override void OnUpdate()
        {
            if (minRushTimer.IsDone && !bodyDetector.Detect())
            {
                EnterStateOfType<BanditNavigationState>();
                return;
            }

            StandardPathFollow();
        }

        private void UpdatePath()
        {
            StandardPathUpdate();

            pathUpdateTimer.Start(enemy.PathUpdateRate);
        }
    }
}
