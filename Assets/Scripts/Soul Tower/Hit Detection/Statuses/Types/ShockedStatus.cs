using Shears;
using System;
using UnityEngine;

namespace SoulTower.HitDetection
{
    [Serializable]
    public struct ShockedStatus : IStatus<ShockedStatus>
    {
#if UNITY_EDITOR
#pragma warning disable CS0414
        [SerializeField, ReadOnly] private string name;

        public string Name { readonly get => name; set => name = value; }
#pragma warning restore CS0414
#endif

        [SerializeField, Min(0.01f)] private float duration;

        readonly ShockedStatus IStatus<ShockedStatus>.Value => this;
        public readonly bool IsUnique => true;
        public readonly float Duration => duration;

        public ShockedStatus(float duration)
        {
#if UNITY_EDITOR
            name = "Shocked Status";
#endif

            this.duration = duration;
        }
    }
}
