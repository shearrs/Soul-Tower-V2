using Shears.StateMachineGraphs;
using UnityEngine;

namespace SoulTower.Enemies
{
    public abstract class EnemyState : State
    {
        private StateMachine stateMachine;

        public void Initialize(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        protected void EnterState(EnemyState state) => stateMachine.EnterState(state);

        protected void EnterStateOfType<T>() where T : EnemyState => stateMachine.EnterStateOfType<T>();

        protected bool IsInStateOfType<T>() where T : EnemyState => stateMachine.IsInStateOfType<T>();
    }
}
