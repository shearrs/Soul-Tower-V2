using System;
using UnityEngine;

namespace SoulTower.HitDetection
{
    public interface IStatusReceiver 
    {
        public Guid Apply<TStatus>(TStatus status) where TStatus : IStatus
        {
            if (this is IStatusReceiver<TStatus> typedReceiver)
                return typedReceiver.Apply(status);
            else
                return Guid.Empty;
        }
    }

    public interface IStatusReceiver<T> : IStatusReceiver where T : IStatus
    {
        public Guid Apply(T status);
    }
}
