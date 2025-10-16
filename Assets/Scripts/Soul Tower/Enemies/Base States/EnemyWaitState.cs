using UnityEngine;

namespace SoulTower.Enemies
{
    public class EnemyWaitState : EnemyState
    {
        private readonly SpeedAnimation animIdle;

        public EnemyWaitState(SpeedAnimation animIdle)
        {
            this.animIdle = animIdle;

            Name = "Wait State";
        }

        protected override void OnEnter()
        {
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
