using UnityEngine;

namespace SoulTower.HitDetection
{
    public interface ISlowStatus : IStatus
    {
        public float SlowPercentage { get; }
    }
}
