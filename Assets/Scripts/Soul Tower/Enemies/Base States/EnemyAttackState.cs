using Shears;
using Shears.Logging;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class EnemyAttackState : EnemyState
    {
        private const float ATTACK_DELAY = 0.5f;
        private const float DAMAGE_DELAY = 0.5f;

        private readonly Timer delayTimer = new(ATTACK_DELAY);
        private readonly Timer damageTimer = new(DAMAGE_DELAY);
        private readonly Enemy enemy;
        private readonly IEnemyAnimation animWindup;
        private readonly IEnemyAnimation animAttack;
        private readonly EnemyState returnState;

        public EnemyAttackState(Enemy enemy, IEnemyAnimation animWindup, IEnemyAnimation animAttack, EnemyState returnState)
        {
            Name = "Attack State";

            this.enemy = enemy;
            this.animWindup = animWindup;
            this.animAttack = animAttack;
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

            delayTimer.Start();
            delayTimer.Completed += StartDamageTimer;
        }

        protected override void OnExit()
        {
            delayTimer.Stop();
            damageTimer.Stop();

            delayTimer.Completed -= StartDamageTimer;
            damageTimer.Completed -= StartDamageTimer;
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

            SetAnimationSpeed(animWindup.Speed);
            CrossFade(animWindup, 0.1f);

            // do a timer for the windup

            damageTimer.Start();
            damageTimer.Completed += OnDamageTimerCompleted;
        }

        private void OnDamageTimerCompleted()
        {
            if (!enemy.CurrentRoom.HasCatalyst)
            {
                EnterState(returnState);
                return;
            }

            enemy.CurrentRoom.Catalyst.Damage();
        }
    }
}
