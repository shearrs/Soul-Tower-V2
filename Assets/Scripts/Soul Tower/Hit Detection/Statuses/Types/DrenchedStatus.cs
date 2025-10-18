using Shears;
using System;
using UnityEngine;

namespace SoulTower.HitDetection
{
    [Serializable]
    public struct DrenchedStatus : IStatus<DrenchedStatus>, ISlowStatus
    {
        private const float WET_DURATION = 10.0f;
        private const float SLOW_PERCENTAGE = 0.5f;

#if UNITY_EDITOR
#pragma warning disable CS0414
        [SerializeField, ReadOnly] private string name;

        public string Name { readonly get => name; set => name = value; }
#pragma warning restore CS0414
#endif

        [SerializeField, Min(0.01f)] private float duration;
        private readonly Guid id;

        readonly DrenchedStatus IStatus<DrenchedStatus>.Value => this;
        public readonly Guid ID => id;
        public readonly bool IsUnique => true;
        public readonly float Duration => duration;
        public readonly float SlowPercentage => SLOW_PERCENTAGE;
        public readonly float WetDuration => WET_DURATION;

        public DrenchedStatus(float duration)
        {
#if UNITY_EDITOR
            name = "Drenched Status";
#endif

            id = Guid.NewGuid();
            this.duration = duration;
        }
    }
}
