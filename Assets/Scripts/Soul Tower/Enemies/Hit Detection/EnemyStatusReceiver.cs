using Shears;
using Shears.Logging;
using SoulTower.HitDetection;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace SoulTower.Enemies
{
    public class EnemyStatusReceiver : SHMonoBehaviourLogger, 
        IStatusReceiver<SlowStatus>, IStatusReceiver<WetStatus>, IStatusReceiver<DrenchedStatus>,
        IStatusReceiver<ColdStatus>, IStatusReceiver<SlimedStatus>, IStatusReceiver<ShockedStatus>
    {
        // status that overrides -> status that gets overridden
        private static readonly Dictionary<Type, Type> OVERRIDE_STATUSES = new()
        {
            { typeof(DrenchedStatus), typeof(WetStatus) }
        };

        // status that gets overriden -> status that overrides
        private static readonly Dictionary<Type, Type> STATUS_OVERRIDES = new()
        {
            { typeof(WetStatus), typeof(DrenchedStatus) }
        };

#if UNITY_EDITOR
        [SerializeReference] private List<IStatus> displayStatuses;
#endif
        private Enemy enemy;

        private Dictionary<Type, Dictionary<Guid, IStatusApplication>> statuses;
        private Dictionary<Guid, IStatusApplication> slows;

        private void Awake()
        {
            if (!TryGetComponent(out enemy))
                enemy = GetComponentInParent<Enemy>();

            if (enemy == null)
            {
                SHLogger.Log($"{nameof(EnemyStatusReceiver)} needs to be attached or a child of an {nameof(Enemy)}");
                return;
            }

            slows = DictionaryPool<Guid, IStatusApplication>.Get();
            statuses = DictionaryPool<Type, Dictionary<Guid, IStatusApplication>>.Get();
        }

        private void OnDestroy()
        {
            slows.Clear();
            DictionaryPool<Guid, IStatusApplication>.Release(slows);

            foreach (var dict in statuses.Values)
            {
                foreach (var application in dict.Values)
                    EnemyTimerPool.Release(application.Timer);

                dict.Clear();
                DictionaryPool<Guid, IStatusApplication>.Release(dict);
            }
        }

        #region Generic Applying
        public void Apply(IStatus status)
        {
            Action applyAction;

            applyAction = status switch
            {
                SlowStatus slow => () => Apply(slow),
                WetStatus wet => () => Apply(wet),
                DrenchedStatus drenched => () => Apply(drenched),
                ColdStatus cold => () => Apply(cold),
                SlimedStatus slimed => () => Apply(slimed),
                ShockedStatus shocked => () => Apply(shocked),
                _ => () => Log($"{nameof(EnemyStatusReceiver)} does not support status type {status.GetType().Name}!", SHLogLevels.Warning)
            };

            applyAction();
        }

        private void Apply<TStatus>(IStatusApplication<TStatus> application) where TStatus : IStatus
        {
            var statusType = typeof(TStatus);

            if (OVERRIDE_STATUSES.ContainsKey(statusType)) // if this status can override another status
            {
                var overridableType = OVERRIDE_STATUSES[statusType];

                if (statuses.TryGetValue(overridableType, out var overridableStatusDict))
                {
                    foreach (var overridableStatus in overridableStatusDict.Values)
                    {
                        Reverse(overridableStatus.Status);

#if UNITY_EDITOR
                        displayStatuses.Remove(overridableStatus.Status);
#endif
                    }

                    // manually remove overridable status
                    overridableStatusDict.Clear();
                    statuses.Remove(overridableType);
                    ReleaseStatusDict(overridableStatusDict);
                }
            }
            else if (STATUS_OVERRIDES.ContainsKey(statusType)) // if this status is already overridden
            {
                var overriderType = STATUS_OVERRIDES[statusType];

                if (statuses.ContainsKey(overriderType))
                    return;
            }

            if (application.Status.IsUnique && statuses.TryGetValue(statusType, out var dict))
            {
                if (dict.Count == 0)
                {
                    Log("Status dictionary has no entries!", SHLogLevels.Error);
                    return;
                }
                else if (dict.Count > 1)
                    Log("Unique status dictionary has more than one entry!", SHLogLevels.Error);

                foreach (var value in dict.Values)
                {
                    if (value.HasTimer)
                        value.Timer.Restart();
                }

                return;
            }

            if (statuses.TryGetValue(statusType, out var statusDict))
            {
                if (statusDict.ContainsKey(application.ID))
                {
                    if (application.HasTimer)
                        application.Timer.Restart();

                    return;
                }
            }

            AddStatus(application);

            if (application.Status is ISlowStatus)
            {
                slows.Add(application.ID, application);
                UpdateSlows();
            }

            if (application.HasTimer)
            {
                application.Timer.Start();
                application.Timer.Completed += endStatus;
            }

            void endStatus()
            {
                RemoveStatus(application);
                Reverse(application.TypedStatus);
                ReleaseTimer(application.Timer);

                if (application.Status is ISlowStatus)
                {
                    slows.Remove(application.ID);
                    UpdateSlows();
                }
            }
        }

        private void Reverse<TStatus>(TStatus status) where TStatus : IStatus
        {
            if (this is IStatusReceiver<TStatus> typedReceiver)
                typedReceiver.Reverse(status);
        }
        #endregion

        #region Slow
        public void Apply(SlowStatus status)
        {
            var timer = GetTimer();
            timer.Time = status.Duration;
            var application = new StatusApplication<SlowStatus>(status, timer);

            Apply(application);
        }

        public void Reverse(SlowStatus status)
        {
        }
        #endregion

        #region Wet
        public void Apply(WetStatus status)
        {
            var timer = GetTimer();
            timer.Time = status.Duration;
            var application = new StatusApplication<WetStatus>(status, timer);

            Apply(application);

            enemy.StatusFlags.IsWet = true;
        }

        public void Reverse(WetStatus status)
        {
            if (!statuses.ContainsKey(typeof(WetStatus)))
                enemy.StatusFlags.IsWet = false;
        }
        #endregion

        #region Drenched
        public void Apply(DrenchedStatus status)
        {
            var timer = GetTimer();
            timer.Time = status.Duration;

            var application = new StatusApplication<DrenchedStatus>(status, timer);

            Apply(application);
            timer.Completed += () => Apply(new WetStatus(status.WetDuration));

            enemy.StatusFlags.IsWet = true;
        }

        public void Reverse(DrenchedStatus status)
        {
            if (!statuses.ContainsKey(typeof(WetStatus)) && !statuses.ContainsKey(typeof(DrenchedStatus)))
                enemy.StatusFlags.IsWet = false;
        }
        #endregion

        #region Cold
        public void Apply(ColdStatus status)
        {
            var timer = GetTimer();
            timer.Time = status.Duration;

            var application = new StatusApplication<ColdStatus>(status, timer);

            Apply(application);

            enemy.StatusFlags.IsCold = true;
        }

        public void Reverse(ColdStatus status)
        {
            if (!statuses.ContainsKey(typeof(ColdStatus)))
                enemy.StatusFlags.IsCold = false;
        }
        #endregion

        #region Slimed
        public void Apply(SlimedStatus status)
        {
            var timer = GetTimer();
            timer.Time = status.Duration;

            var application = new StatusApplication<SlimedStatus>(status, timer);

            Apply(application);

            enemy.StatusFlags.IsSlimed = true;
        }

        public void Reverse(SlimedStatus status)
        {
            if (!statuses.ContainsKey(typeof(SlimedStatus)))
                enemy.StatusFlags.IsSlimed = false;
        }
        #endregion

        #region Shocked
        public void Apply(ShockedStatus status)
        {
            var timer = GetTimer();
            timer.Time = status.Duration;

            var application = new StatusApplication<ShockedStatus>(status, timer);

            Apply(application);

            enemy.StatusFlags.IsShocked = true;
        }

        public void Reverse(ShockedStatus status)
        {
            if (!statuses.ContainsKey(typeof(ShockedStatus)))
                enemy.StatusFlags.IsShocked = false;
        }
        #endregion

        #region Adding and Removing Statuses
        private void UpdateSlows()
        {
            float percentage = 1.0f;

            foreach (var slow in slows.Values)
            {
                if (slow.Status is ISlowStatus slowStatus)
                    percentage *= 1.0f - slowStatus.SlowPercentage;
            }

            percentage = Mathf.Max(percentage, 0.0f);

            enemy.SetMoveSpeedPercentage(percentage);
        }

        private void AddStatus<TStatus>(IStatusApplication<TStatus> application) where TStatus : IStatus
        {
            var type = typeof(TStatus);

            if (statuses.TryGetValue(type, out var dict))
                dict.Add(application.ID, application);
            else
            {
                dict = GetStatusDict();
                dict.Add(application.ID, application);
                statuses[type] = dict;

#if UNITY_EDITOR
                displayStatuses.Add(application.Status);
#endif
            }
        }

        private void RemoveStatus<TStatus>(IStatusApplication<TStatus> application) where TStatus : IStatus
        {
            var type = typeof(TStatus);

            if (statuses.TryGetValue(type, out var dict))
            {
                dict.Remove(application.ID);

                if (dict.Count == 0)
                {
                    statuses.Remove(type);
                    dict.Clear();

                    ReleaseStatusDict(dict);

#if UNITY_EDITOR
                    displayStatuses.Remove(application.Status);
#endif
                }
            }
        }
        #endregion

        #region Pools
        private Timer GetTimer() => EnemyTimerPool.Get();

        private Dictionary<Guid, IStatusApplication> GetStatusDict() => DictionaryPool<Guid, IStatusApplication>.Get();

        private void ReleaseStatusDict(Dictionary<Guid, IStatusApplication> dict)
        {
            dict.Clear();
            DictionaryPool<Guid, IStatusApplication>.Release(dict);
        }

        private void ReleaseTimer(Timer timer) => EnemyTimerPool.Release(timer);
        #endregion
    }
}
