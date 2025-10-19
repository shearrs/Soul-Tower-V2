using Shears;
using System;
using UnityEngine;

namespace SoulTower.HitDetection
{
    [Serializable]
    public struct SlimedStatus : IStatus<SlimedStatus>, ISlowStatus
    {
#if UNITY_EDITOR
#pragma warning disable CS0414
        [SerializeField, ReadOnly] private string name;

        public string Name { readonly get => name; set => name = value; }
#pragma warning restore CS0414
#endif

        [SerializeField, Min(0.01f)] private float duration;

        public readonly bool IsUnique => true;
        public readonly float Duration => duration;
        public readonly float SlowPercentage => 0.5f;

        readonly SlimedStatus IStatus<SlimedStatus>.Value => this;

        public SlimedStatus(float duration)
        {
#if UNITY_EDITOR
            name = "Slimed Status";
#endif

            this.duration = duration;
        }
    }
}
