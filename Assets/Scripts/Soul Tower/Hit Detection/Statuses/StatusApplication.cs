using Shears;
using System;
using UnityEngine;

namespace SoulTower.HitDetection
{
    public interface IStatusApplication
    {
        public Guid ID => Status.ID;
        public IStatus Status { get; }
        public bool HasTimer { get; }
        public Timer Timer { get; }
    }

    public interface IStatusApplication<TStatus> : IStatusApplication where TStatus : IStatus
    {
        public TStatus TypedStatus { get; }
    }

    public readonly struct StatusApplication<TStatus> : IStatusApplication<TStatus> where TStatus : IStatus
    {
        private readonly TStatus status;
        private readonly Timer timer;

        public readonly IStatus Status => status;
        public readonly bool HasTimer => timer != null;
        public readonly Timer Timer => timer;
        public TStatus TypedStatus => status;

        public StatusApplication(TStatus status)
        {
            this.status = status;
            timer = null;
        }

        public StatusApplication(TStatus status, Timer timer)
        {
            this.status = status;
            this.timer = timer;
        }
    }
}
