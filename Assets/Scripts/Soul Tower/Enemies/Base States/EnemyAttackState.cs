using Shears;
using Shears.Logging;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class EnemyAttackState : EnemyState
    {
        private const float ATTACK_DELAY = 0.5f;
        private const float DAMAGE_DELAY = 0.35f;

        private readonly Timer windupTimer = new(ATTACK_DELAY);
        private readonly Timer damageTimer = new(DAMAGE_DELAY);
        private readonly Enemy enemy;
        private readonly IEnemyAnimation animWindup;
        private readonly IEnemyAnimation animAttack;
        private readonly IEnemyAnimation animFollowThrough;
        private readonly EnemyState returnState;

        public EnemyAttackState(Enemy enemy, IEnemyAnimation animWindup, IEnemyAnimation animAttack, IEnemyAnimation animFollowThrough, EnemyState returnState)
        {
            Name = "Attack State";

            this.enemy = enemy;
            this.animWindup = animWindup;
            this.animAttack = animAttack;
            this.animFollowThrough = animFollowThrough;
            this.returnState = returnState;
        }

        protected override void OnEnter()
        {
            if (!enemy.CurrentRoom.HasCatalyst)
            {
                Log("Enemy's current room does not have a catalyst!", SHLogLevels.Error, context: enemy);
                EnterState(returnState);

                return;
            }

            if (!IsInAnimation(animWindup))
            {
                SetAnimationSpeed(animWindup.Speed);
                CrossFade(animWindup, 0.1f);
            }

            windupTimer.Start();
            windupTimer.Completed += StartDamageTimer;
            damageTimer.Completed += OnDamageTimerCompleted;
        }

        protected override void OnExit()
        {
            windupTimer.Stop();
            damageTimer.Stop();

            windupTimer.Completed -= StartDamageTimer;
            damageTimer.Completed -= OnDamageTimerCompleted;
        }

        protected override void OnUpdate()
        {
        }

        private void StartDamageTimer()
        {
            if (!enemy.CurrentRoom.HasCatalyst)
            {
                EnterState(returnState);
                return;
            }

            SetAnimationSpeed(animAttack.Speed);
            CrossFade(animAttack, 0.1f);

            damageTimer.Start();
        }

        private void OnDamageTimerCompleted()
        {
            if (!enemy.CurrentRoom.HasCatalyst)
            {
                EnterState(returnState);
                return;
            }

            SetAnimationSpeed(animFollowThrough.Speed);
            CrossFade(animFollowThrough, 0.1f);

            enemy.CurrentRoom.Catalyst.Damage();
        }
    }
}
