using UnityEngine;

namespace SoulTower.Enemies
{
    public class BanditStairsState : EnemyState
    {
        private readonly Bandit bandit;

        public BanditStairsState(Bandit bandit)
        {
            Name = "Bandit Stairs State";

            this.bandit = bandit;
        }

        protected override void OnEnter()
        {
        }

        protected override void OnExit()
        {
            bandit.BeginStairsTimer();
        }

        protected override void OnUpdate()
        {
        }
    }
}
