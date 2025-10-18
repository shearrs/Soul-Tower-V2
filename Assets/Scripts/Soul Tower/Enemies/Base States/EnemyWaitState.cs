using UnityEngine;

namespace SoulTower.Enemies
{
    public class EnemyWaitState : EnemyState
    {
        private readonly IEnemyAnimation animIdle;

        public EnemyWaitState(IEnemyAnimation animIdle)
        {
            this.animIdle = animIdle;

            Name = "Wait State";
        }

        protected override void OnEnter()
        {
            SetAnimationSpeed(animIdle.Speed);
            CrossFade(animIdle, 0.1f);
        }

        protected override void OnExit()
        {
        }

        protected override void OnUpdate()
        {
        }
    }
}
