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
        private readonly IEnemyAnimation animDelay;
        private readonly IEnemyAnimation animAttack;
        private readonly EnemyState returnState;

        public EnemyAttackState(Enemy enemy, IEnemyAnimation animDelay, IEnemyAnimation animAttack, EnemyState returnState)
        { 
            this.enemy = enemy;
            this.animDelay = animDelay;
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

            if (!IsInAnimation(animDelay))
            {
                SetAnimationSpeed(animDelay.Speed);
                CrossFade(animDelay, 0.1f);
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

            SetAnimationSpeed(animDelay.Speed);
            CrossFade(animAttack, 0.1f);

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
