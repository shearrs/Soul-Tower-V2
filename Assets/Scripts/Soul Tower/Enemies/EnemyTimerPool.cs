using Shears;
using UnityEngine;
using UnityEngine.Pool;

namespace SoulTower.Enemies
{
    public static class EnemyTimerPool
    {
        private static ObjectPool<Timer> timerPool;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializePool()
        {
            timerPool = new(Create, actionOnRelease: OnRelease, actionOnDestroy: OnDestroy);
        }

        public static Timer Get() => timerPool.Get();

        public static void Release(Timer timer) => timerPool.Release(timer);

        private static Timer Create() => new();

        private static void OnRelease(Timer timer)
        {
            timer.Stop();
            timer.ClearOnCompletes();
        }

        private static void OnDestroy(Timer timer)
        {
            timer.Stop();
            timer.ClearOnCompletes();
        }
    }
}
