using UnityEngine;

namespace SoulTower.Enemies
{
    public interface IEnemyAnimation
    {
        public int ID { get; }
        public float Speed { get; }
        public int Layer { get; }
    }
}
