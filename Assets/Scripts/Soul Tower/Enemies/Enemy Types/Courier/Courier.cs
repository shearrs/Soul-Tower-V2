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
        private static readonly int TIRED_WALK_BLEND_ID = Animator.StringToHash("tired_walk_blend");

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
        [SerializeField] private AnimationClip animIdle;
        [SerializeField] private AnimationClip animWalk;
        [SerializeField] private AnimationClip animTired;
        [SerializeField] private AnimationClip animWindup;
        [SerializeField] private AnimationClip animAttack;

        private readonly Timer decelerateCooldown = new();
        private EnemyState[] states;
        private Enemy enemy;
        private bool isAccelerating;
        private bool isDecelerating;

        public bool IsAccelerating => isAccelerating;
        public bool IsDecelerating => isDecelerating;

        private void Awake()
        {
            enemy = GetComponent<Enemy>();

            var walkTiredBlend = new MoveSpeedAnimation(enemy, TIRED_WALK_BLEND_ID);
            var animWalk = new MoveSpeedAnimation(enemy, this.animWalk);
            var animIdle = new FixedSpeedAnimation(this.animIdle);
            var animTired = new FixedSpeedAnimation(this.animTired);
            var animWindup = new FixedSpeedAnimation(this.animWindup);
            var animAttack = new FixedSpeedAnimation(this.animAttack);

            var waitState = new EnemyWaitState(animIdle);
            var followPathState = new EnemyFollowPathState(enemy, animWalk);
            var navigationState = new EnemyNavigationState(frontDetector, bodyDetector, waitState, followPathState);
            var stopChanceState = new CourierStopChanceState(this, stopChance);
            var decelerationState = new CourierDecelerationState(enemy, this, decelerationSpeed, walkTiredBlend);
            var accelerationState = new CourierAccelerationState(enemy, this, accelerationSpeed, walkTiredBlend);
            var restState = new CourierRestState(restDuration, animTired);
            var stairsState = new EnemyStairsState(enemy, animWalk, followPathState);
            var attackState = new EnemyAttackState(enemy, animWindup, animAttack, animIdle, followPathState);
            var catalystState = new EnemyCatalystState(enemy, animWalk, followPathState, attackState);
            var entranceState = new EnemyEntranceState(enemy, animWalk, followPathState);

            navigationState.AddSubState(waitState);
            navigationState.AddSubState(followPathState);
            navigationState.DefaultSubState = followPathState;

            followPathState.AddSubState(stopChanceState);
            followPathState.AddSubState(decelerationState);
            followPathState.AddSubState(accelerationState);
            followPathState.DefaultSubState = stopChanceState;

            states = new EnemyState[]
            {
                entranceState,
                waitState,
                followPathState,
                navigationState,
                stopChanceState,
                decelerationState,
                accelerationState,
                restState,
                stairsState,
                catalystState,
                attackState
            };

            foreach (var state in states)
                state.Initialize(enemy, stateMachine, pathfinder);

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

        public void BeginAccelerating()
        {
            isAccelerating = true;
        }

        public void EndAccelerating()
        {
            isAccelerating = false;
        }

        public void BeginDecelerating()
        {
            isDecelerating = true;

            if (decelerateCooldown.IsDone)
                decelerateCooldown.Completed += OnDecelerateComplete;

            decelerateCooldown.Restart(stopChanceCooldown);
        }

        public void EndDecelerating()
        {
            isDecelerating = false;
        }

        public bool IsStopOnCooldown() => !decelerateCooldown.IsDone;

        public void AddStopCooldownEvent(Action action) => decelerateCooldown.Completed += action;

        public void RemoveStopCooldownEvent(Action action) => decelerateCooldown.Completed -= action;

        private void OnDecelerateComplete()
        {
            decelerateCooldown.ClearOnCompletes();
        }
    }
}
