using UnityEngine;

namespace SoulTower.HitDetection
{
    public readonly struct SlowStatus : IStatus<SlowStatus>
    {
        private readonly float percentage;
        private readonly float duration;

        public readonly float Percentage => percentage;
        public readonly float Duration => duration;

        SlowStatus IStatus<SlowStatus>.Value => this;

        public SlowStatus(float percentage, float duration)
        {
            this.percentage = percentage;
            this.duration = duration;
        }
    }
}
