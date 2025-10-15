using SoulTower.HitDetection;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class EnemyStatusReceiver : MonoBehaviour, IStatusReceiver<SlowStatus>
    {
        [SerializeField] private Enemy enemy;

        public void Apply(SlowStatus status)
        {
            enemy.MoveSpeed = status.Percentage * enemy.BaseMoveSpeed;

            // factor in duration
        }
    }
}
