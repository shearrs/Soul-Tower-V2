using Shears;
using System;
using UnityEngine;

namespace SoulTower.HitDetection
{
    [Serializable]
    public struct WetStatus : IStatus<WetStatus>, ISlowStatus
    {
        private const float SLOW_PERCENTAGE = 0.25f;

#if UNITY_EDITOR
#pragma warning disable CS0414
        [SerializeField, ReadOnly] private string name;

        public string Name { readonly get => name; set => name = value; }
#pragma warning restore CS0414
#endif

        [SerializeField, Min(0.01f)] private float duration;
        private readonly Guid id;

        readonly WetStatus IStatus<WetStatus>.Value => this;
        public readonly Guid ID => id;
        public readonly bool IsUnique => true;
        public readonly float Duration => duration;
        public readonly float SlowPercentage => SLOW_PERCENTAGE;

        public WetStatus(float duration)
        {
#if UNITY_EDITOR
            name = "Wet Status";
#endif

            id = Guid.NewGuid();
            this.duration = duration;
        }
    }
}
