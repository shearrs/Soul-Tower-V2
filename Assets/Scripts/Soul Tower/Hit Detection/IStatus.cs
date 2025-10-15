using Shears;
using UnityEngine;

namespace SoulTower.HitDetection
{
    public interface IStatus { }

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
