using System.Collections;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class CourierSlowingState : EnemyState
    {
        private const float SLOW_DURATION = 1.0f;

        private readonly EnemyStatusReceiver statusReceiver;

        public CourierSlowingState(EnemyStatusReceiver statusReceiver)
        {
            this.statusReceiver = statusReceiver;
        }

        protected override void OnEnter()
        {
        }

        protected override void OnExit()
        {
        }

        protected override void OnUpdate()
        {
        }
    }
}
