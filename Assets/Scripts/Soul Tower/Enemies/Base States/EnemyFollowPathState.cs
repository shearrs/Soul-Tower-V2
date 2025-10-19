using Shears;

namespace SoulTower.Enemies
{
    public class EnemyFollowPathState : EnemyState
    {
        private readonly Timer updatePathTimer;
        private readonly IEnemyAnimation animWalk;

        public EnemyFollowPathState(Enemy enemy, IEnemyAnimation animWalk)
        {
            Name = "Follow Path State";

            this.animWalk = animWalk;
            updatePathTimer = new(enemy.PathUpdateRate);
        }

        ~EnemyFollowPathState()
        {
            DeregisterNodes();
        }

        protected override void OnEnter()
        {
            updatePathTimer.Start();
            updatePathTimer.Completed += UpdatePath;

            UpdatePath();

            SetAnimationSpeed(animWalk.Speed);
            CrossFade(animWalk, 0.1f);
        }

        protected override void OnExit()
        {
            updatePathTimer.Stop();
            updatePathTimer.Completed -= UpdatePath;

            DeregisterNodes();
        }

        protected override void OnUpdate()
        {
            Move();
        }

        private void UpdatePath()
        {
            StandardPathUpdate();
        }

        private void Move()
        {
            StandardPathFollow();
        }
    }
}
