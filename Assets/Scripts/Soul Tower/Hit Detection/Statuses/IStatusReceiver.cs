using System;
using UnityEngine;

namespace SoulTower.HitDetection
{
    public interface IStatusReceiver 
    {
    }

    public interface IStatusReceiver<T> : IStatusReceiver where T : IStatus
    {
        public void Apply(T status);
        public void Reverse(T status);
    }
}
