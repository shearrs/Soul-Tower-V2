using Shears;
using Shears.Detection;
using Shears.StateMachineGraphs;
using System;
using UnityEngine;

namespace SoulTower.Enemies
{
    [RequireComponent(typeof(Enemy))]
    public class Courier : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private StateMachine stateMachine;
        [SerializeField] private EnemyPathfinder pathfinder;
        [SerializeField] private AreaDetector3D frontDetector;
        [SerializeField] private AreaDetector3D bodyDetector;

        [Header("Settings")]
        [SerializeField] private float decelerationSpeed = 1.0f;
        [SerializeField] private float accelerationSpeed = 1.0f;
        [SerializeField, Range(0.0f, 1.0f)] private float stopChance = 1.0f;
        [SerializeField, Min(0.0f)] private float stopChanceCooldown = 1.0f;
        [SerializeField, Min(0.0f)] private float restDuration = 2.0f;

        [Header("Animations")]
        [SerializeField] private float walkPlaybackSpeed = 1.25f;
        [SerializeField] private AnimationClip animIdle;
        [SerializeField] private AnimationClip animWalk;

        private readonly Timer stopCooldown = new();
        private EnemyState[] states;
        private Enemy enemy;
        private bool isStopping;

        public bool IsStopping => isStopping;

        private void Awake()
        {
            enemy = GetComponent<Enemy>();

            var walkAnimation = new SpeedAnimation(animWalk, walkPlaybackSpeed);

            var waitState = new EnemyWaitState(animIdle);
            var followPathState = new EnemyFollowPathState(enemy, pathfinder, walkAnimation);
            var navigationState = new EnemyNavigationState(frontDetector, bodyDetector, waitState, followPathState);
            var stopChanceState = new CourierStopChanceState(this, stopChance);
            var decelerationState = new CourierDecelerationState(enemy, this, decelerationSpeed, walkAnimation);
            var accelerationState = new CourierAccelerationState(enemy, accelerationSpeed, walkAnimation);
            var restState = new CourierRestState(restDuration, animIdle);
            var stairsState = new EnemyStairsState(enemy, pathfinder, navigationState);
            var catalystState = new EnemyCatalystState(frontDetector);
            var entranceState = new EnemyEntranceState(enemy, pathfinder, walkAnimation, navigationState);

            navigationState.AddSubState(waitState);
            navigationState.AddSubState(followPathState);

            followPathState.AddSubState(stopChanceState);
            followPathState.AddSubState(decelerationState);
            followPathState.AddSubState(accelerationState);

            followPathState.DefaultSubState = stopChanceState;

            states = new EnemyState[]
            {
                entranceState,
                navigationState,
                waitState,
                followPathState,
                stopChanceState,
                decelerationState,
                accelerationState,
                restState,
                stairsState,
                catalystState
            };

            foreach (var state in states)
                state.Initialize(enemy, stateMachine);

            stateMachine.AddStates(states);
        }

        private void OnEnable()
        {
            enemy.Spawned += OnSpawned;
        }

        private void OnDisable()
        {
            enemy.Spawned -= OnSpawned;
        }

        private void OnSpawned()
        {
            stateMachine.EnterStateOfType<EnemyEntranceState>();
        }

        public void BeginStopping()
        {
            isStopping = true;

            if (stopCooldown.IsDone)
                stopCooldown.Completed += OnStopCooldownComplete;

            stopCooldown.Restart(stopChanceCooldown);
        }

        public void EndStopping()
        {
            isStopping = false;
        }

        public bool IsStopOnCooldown() => !stopCooldown.IsDone;

        public void AddStopCooldownEvent(Action action) => stopCooldown.Completed += action;

        public void RemoveStopCooldownEvent(Action action) => stopCooldown.Completed -= action;

        private void OnStopCooldownComplete()
        {
            stopCooldown.ClearOnCompletes();
        }
    }
}
