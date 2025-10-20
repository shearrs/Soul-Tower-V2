using Shears.Detection;
using Shears.StateMachineGraphs;
using UnityEngine;

namespace SoulTower.Enemies
{
    [RequireComponent(typeof(Enemy))]
    public class Defender : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private StateMachine stateMachine;
        [SerializeField] private EnemyPathfinder pathfinder;
        [SerializeField] private DefenderShield shield;
        [SerializeField] private AreaDetector3D frontDetector;
        [SerializeField] private AreaDetector3D bodyDetector;

        [Header("Animations")]
        [SerializeField] private AnimationClip animIdle;
        [SerializeField] private AnimationClip animWalk;

        private Enemy enemy;
        private EnemyState[] states;

        internal bool IsSwappingShield { get; set; }

        private void Awake()
        {
            enemy = GetComponent<Enemy>();

            var animIdle = new FixedSpeedAnimation(this.animIdle);
            var animWalk = new MoveSpeedAnimation(enemy, this.animWalk);

            var waitState = new EnemyWaitState(animIdle);
            var followPathState = new EnemyFollowPathState(enemy, animWalk);
            var navigationState = new DefenderNavigationState(this, frontDetector, bodyDetector);
            var shieldSwapState = new DefenderSwapShieldState(this, shield);
            var entranceState = new EnemyEntranceState(enemy, animWalk, shieldSwapState);
            var stairsState = new EnemyStairsState(enemy, animWalk, shieldSwapState);
            var attackState = new EnemyAttackState(enemy, animIdle, animWalk, shieldSwapState);
            var catalystState = new EnemyCatalystState(enemy, animWalk, shieldSwapState, attackState);
            var blockState = new DefenderBlockState(animIdle);

            shieldSwapState.AddSubState(navigationState);
            shieldSwapState.DefaultSubState = navigationState;

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
                attackState,
                shieldSwapState,
                blockState
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
    }
}
