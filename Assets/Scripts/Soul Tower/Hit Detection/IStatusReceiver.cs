using UnityEngine;

namespace SoulTower.HitDetection
{
    public interface IStatusReceiver 
    {
        public void Apply<TStatus>(TStatus status) where TStatus : IStatus
        {
            if (this is IStatusReceiver<TStatus> typedReceiver)
                typedReceiver.Apply(status);
        }
    }

    public interface IStatusReceiver<T> : IStatusReceiver where T : IStatus
    {
        public void Apply(T status);
    }
}
