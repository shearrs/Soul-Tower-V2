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

        [Header("Detectors")]
        [SerializeField] private AreaDetector3D climbDetector;

        [Header("Animations")]
        [SerializeField] private AnimationClip animIdle;
        [SerializeField] private AnimationClip animWalk;
        [SerializeField] private AnimationClip animThrowHook;
        [SerializeField] private AnimationClip animClimb;

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

            var followPathState = new EnemyFollowPathState(enemy, animWalk);
            var stairsState = new EnemyStairsState(enemy, animWalk, followPathState);
            var attackState = new EnemyAttackState(enemy, animIdle, animWalk, followPathState);
            var catalystState = new EnemyCatalystState(enemy, animWalk, followPathState, attackState);
            var entranceState = new ClimberEntranceState(enemy, this, pathfinder, climbDetector, animWalk);
            var prepareState = new ClimberPrepareState(hook, animThrowHook);
            var climbState = new ClimberClimbState(enemy, this, hook, animIdle, animClimb, animWalk, animIdle);

            states = new EnemyState[]
            {
                entranceState,
                followPathState,
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
