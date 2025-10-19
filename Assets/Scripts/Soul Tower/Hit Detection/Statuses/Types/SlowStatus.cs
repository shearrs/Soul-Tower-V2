using Shears;
using System;
using UnityEngine;

namespace SoulTower.HitDetection
{
    [Serializable]
    public struct SlowStatus : IStatus<SlowStatus>, ISlowStatus
    {
#if UNITY_EDITOR
#pragma warning disable CS0414
        [SerializeField, ReadOnly] private string name;

        public string Name { readonly get => name; set => name = value; }
#pragma warning restore CS0414
#endif

        [SerializeField, Min(0.01f)] private float duration;
        [SerializeField, Range(0.0f, 1.0f)] private float slowPercentage;

        public readonly bool IsUnique => false;
        public readonly float Duration => duration;
        public readonly float SlowPercentage => slowPercentage;

        readonly SlowStatus IStatus<SlowStatus>.Value => this;

        public SlowStatus(float duration, float percentage)
        {
#if UNITY_EDITOR
            name = "Slow Status";
#endif

            slowPercentage = percentage;
            this.duration = duration;
        }
    }
}
