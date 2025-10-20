using Shears.Detection;
using Shears.StateMachineGraphs;
using SoulTower.Towers;
using UnityEngine;

namespace SoulTower.Enemies
{
    [RequireComponent(typeof(Enemy))]
    public class Climber : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private StateMachine stateMachine;
        [SerializeField] private EnemyPathfinder pathfinder;
        [SerializeField] private ClimberHook hook;
        [SerializeField] private AreaDetector3D frontDetector;
        [SerializeField] private AreaDetector3D bodyDetector;

        [Header("Detectors")]
        [SerializeField] private AreaDetector3D climbDetector;

        [Header("Animations")]
        [SerializeField] private AnimationClip animIdle;
        [SerializeField] private AnimationClip animWalk;
        [SerializeField] private AnimationClip animThrowHook;
        [SerializeField] private AnimationClip animClimb;
        [SerializeField] private AnimationClip animWindup;
        [SerializeField] private AnimationClip animAttack;

        private Enemy enemy;
        private EnemyState[] states;

        internal WallOpening TargetOpening { get; set; }

        private void Awake()
        {
            enemy = GetComponent<Enemy>();

            var animWalk = new MoveSpeedAnimation(enemy, this.animWalk);
            var animIdle = new FixedSpeedAnimation(this.animIdle);
            var animThrowHook = new FixedSpeedAnimation(this.animThrowHook, 2.0f);
            var animClimb = new FixedSpeedAnimation(this.animClimb);
            var animWindup = new FixedSpeedAnimation(this.animWindup);
            var animAttack = new FixedSpeedAnimation(this.animAttack);

            var waitState = new EnemyWaitState(animIdle);
            var followPathState = new EnemyFollowPathState(enemy, animWalk);
            var navigationState = new EnemyNavigationState(frontDetector, bodyDetector, waitState, followPathState);
            var stairsState = new EnemyStairsState(enemy, animWalk, navigationState);
            var attackState = new EnemyAttackState(enemy, animWindup, animAttack, animIdle, navigationState);
            var catalystState = new EnemyCatalystState(enemy, animWalk, navigationState, attackState);
            var entranceState = new ClimberEntranceState(enemy, this, pathfinder, climbDetector, animWalk);
            var prepareState = new ClimberPrepareState(hook, animThrowHook);
            var climbState = new ClimberClimbState(enemy, this, hook, animIdle, animClimb, animWalk, animIdle);

            navigationState.AddSubState(waitState);
            navigationState.AddSubState(followPathState);
            navigationState.DefaultSubState = followPathState;

            states = new EnemyState[]
            {
                entranceState,
                waitState,
                followPathState,
                navigationState,
                stairsState,
                catalystState,
                prepareState,
                climbState,
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
            stateMachine.EnterStateOfType<ClimberEntranceState>();
        }
    }
}
