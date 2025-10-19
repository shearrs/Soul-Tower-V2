using Shears;
using System;
using UnityEngine;

namespace SoulTower.HitDetection
{
    [Serializable]
    public struct ColdStatus : IStatus<ColdStatus>, ISlowStatus
    {
        private const float SLOW_PERCENTAGE = 0.75f;

#if UNITY_EDITOR
#pragma warning disable CS0414
        [SerializeField, ReadOnly] private string name;

        public string Name { readonly get => name; set => name = value; }
#pragma warning restore CS0414
#endif

        [SerializeField, Min(0.01f)] private float duration;

        readonly ColdStatus IStatus<ColdStatus>.Value => this;
        public readonly bool IsUnique => true;
        public readonly float Duration => duration;
        public readonly float SlowPercentage => SLOW_PERCENTAGE;

        public ColdStatus(float duration)
        {
#if UNITY_EDITOR
            name = "Cold Status";
#endif

            this.duration = duration;
        }
    }
}
