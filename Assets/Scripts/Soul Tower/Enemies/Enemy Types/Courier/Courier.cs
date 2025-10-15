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

        private readonly Timer stopCooldown = new();
        private EnemyState[] states;
        private Enemy enemy;
        private bool isStopping;

        public bool IsStopping => isStopping;

        private void Awake()
        {
            enemy = GetComponent<Enemy>();

            EnemyState waitState = new EnemyWaitState();
            EnemyState followPathState = new EnemyFollowPathState(enemy, pathfinder);
            EnemyState navigationState = new EnemyNavigationState(frontDetector, bodyDetector, waitState, followPathState);
            EnemyState stopChanceState = new CourierStopChanceState(this, stopChance);
            EnemyState decelerationState = new CourierDecelerationState(enemy, this, decelerationSpeed);
            EnemyState accelerationState = new CourierAccelerationState(enemy, accelerationSpeed);

            states = new EnemyState[]
            {
                navigationState,
                waitState,
                followPathState,
                stopChanceState,
                decelerationState,
                accelerationState,
                new CourierRestState(restDuration),
                new EnemyStairsState(enemy.Tower, enemy, pathfinder, navigationState),
                new EnemyCatalystState(frontDetector)
            };

            navigationState.AddSubState(waitState);
            navigationState.AddSubState(followPathState);

            followPathState.AddSubState(stopChanceState);
            followPathState.AddSubState(decelerationState);
            followPathState.AddSubState(accelerationState);

            followPathState.DefaultSubState = stopChanceState;

            foreach (var state in states)
                state.Initialize(enemy, stateMachine);

            stateMachine.AddStates(states);
        }

        private void Start()
        {
            stopCooldown.Start();
            stateMachine.EnterStateOfType<EnemyNavigationState>();
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
