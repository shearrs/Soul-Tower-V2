using Shears.StateMachineGraphs;
using UnityEngine;

namespace SoulTower.Enemies
{
    public abstract class VillagerState : State
    {
        private StateMachine stateMachine;

        public void Initialize(StateMachine stateMachine)
        {
            this.stateMachine = stateMachine;
        }

        protected void EnterStateOfType<T>() where T : VillagerState => stateMachine.EnterStateOfType<T>();

        protected bool IsInStateOfType<T>() where T : VillagerState => stateMachine.IsInStateOfType<T>();
    }
}
