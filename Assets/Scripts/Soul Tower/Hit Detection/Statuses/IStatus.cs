using Shears;
using System;
using UnityEngine;

namespace SoulTower.HitDetection
{
    public interface IStatus
    {
        public bool IsUnique { get; }

#if UNITY_EDITOR
        public string Name { get; set; }
#endif
    }

    public interface IStatus<T> : IStatus where T : IStatus
    {
        public T Value { get; }

        public void Apply(IStatusReceiver receiver)
        {
            if (receiver is IStatusReceiver<T> typedReceiver)
                typedReceiver.Apply(Value);
        }
    }
}
