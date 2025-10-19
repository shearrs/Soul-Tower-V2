using Shears.Signals;
using UnityEngine;

namespace SoulTower.Enemies
{
    public readonly struct EnemyDiedSignal : ISignal
    {
        private readonly Enemy enemy;

        public readonly Enemy Enemy => enemy;

        public EnemyDiedSignal(Enemy enemy)
        {
            this.enemy = enemy;
        }
    }
}
