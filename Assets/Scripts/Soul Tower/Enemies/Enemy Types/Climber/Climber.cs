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
        [SerializeField] private AreaDetector3D frontDetector;
        [SerializeField] private AreaDetector3D bodyDetector;
        [SerializeField] private AreaDetector3D climbDetector;

        [Header("Animations")]
        [SerializeField] private AnimationClip animIdle;
        [SerializeField] private AnimationClip animWalk;

        private Enemy enemy;
        private EnemyState[] states;

        internal WallOpening TargetOpening { get; set; }

        private void Awake()
        {
            enemy = GetComponent<Enemy>();

            var animWalk = new MoveSpeedAnimation(enemy, this.animWalk);
            var animIdle = new FixedSpeedAnimation(this.animIdle);

            var waitState = new EnemyWaitState(animIdle);
            var followPathState = new EnemyFollowPathState(enemy, pathfinder, animWalk);
            var navigationState = new EnemyNavigationState(frontDetector, bodyDetector, waitState, followPathState);
            var stairsState = new EnemyStairsState(enemy, animWalk, navigationState);
            var attackState = new EnemyAttackState(enemy, animIdle, animWalk, navigationState);
            var catalystState = new EnemyCatalystState(enemy, animWalk, navigationState, attackState);
            var entranceState = new ClimberEntranceState(enemy, this, pathfinder, climbDetector, animWalk);
            var prepareState = new ClimberPrepareState(animIdle);
            var climbState = new ClimberClimbState(enemy, this, pathfinder, animIdle, animWalk, animIdle); // TODO: needs climb and fall animations

            navigationState.AddSubState(waitState);
            navigationState.AddSubState(followPathState);

            states = new EnemyState[]
            {
                entranceState,
                navigationState,
                waitState,
                followPathState,
                stairsState,
                catalystState,
                prepareState,
                climbState,
                attackState
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
            stateMachine.EnterStateOfType<ClimberEntranceState>();
        }
    }
}
