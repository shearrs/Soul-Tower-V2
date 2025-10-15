using Shears;
using SoulTower.HitDetection;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace SoulTower.Enemies
{
    public class EnemyStatusReceiver : MonoBehaviour, IStatusReceiver<SlowStatus>
    {
        [SerializeField] private Enemy enemy;

        private readonly Dictionary<Guid, Action> statusCancelers = new();

        private ObjectPool<Timer> timerPool;

        private void Awake()
        {
            timerPool = new(CreateTimer, actionOnRelease: OnReleaseTimer, actionOnDestroy: OnDestroyTimer);
        }

        // current implementation means that successive slows are less effective
        // in order to fix this, we need to keep a collection of the currently applied move speeds and their percentage so we can add to it (of course clamping it)
        public Guid Apply(SlowStatus status)
        {
            enemy.SetMoveSpeed(status.Percentage * enemy.MoveSpeed);

            var timer = timerPool.Get();
            timer.Start(status.Duration);
            var id = Guid.NewGuid();

            void endStatus()
            {
                enemy.SetMoveSpeed(enemy.MoveSpeed / status.Percentage);
                timerPool.Release(timer);

                statusCancelers.Remove(id);
            }

            timer.Completed += endStatus;
            statusCancelers[id] = endStatus;

            return id;
        }

        private Timer CreateTimer() => new();

        private void OnReleaseTimer(Timer timer)
        {
            timer.Stop();
            timer.ClearOnCompletes();
        }

        private void OnDestroyTimer(Timer timer)
        {
            timer.Stop();
            timer.ClearOnCompletes();
        }
    }
}
