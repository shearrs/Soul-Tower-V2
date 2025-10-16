using Shears.Detection;
using Shears.StateMachineGraphs;
using UnityEngine;

namespace SoulTower.Enemies
{
    [RequireComponent(typeof(Enemy))]
    public class Villager : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private StateMachine stateMachine;
        [SerializeField] private EnemyPathfinder pathfinder;
        [SerializeField] private AreaDetector3D frontDetector;
        [SerializeField] private AreaDetector3D bodyDetector;

        [Header("Animations")]
        [SerializeField] private float walkPlaybackSpeed = 1.25f;
        [SerializeField] private AnimationClip animIdle;
        [SerializeField] private AnimationClip animWalk;

        private EnemyState[] states;
        private Enemy enemy;

        private void Awake()
        {
            enemy = GetComponent<Enemy>();

            var walkAnimation = new SpeedAnimation(animWalk, walkPlaybackSpeed);

            var waitState = new EnemyWaitState(animIdle);
            var followPathState = new EnemyFollowPathState(enemy, pathfinder, walkAnimation);
            var navigationState = new EnemyNavigationState(frontDetector, bodyDetector, waitState, followPathState);
            var stairsState = new EnemyStairsState(enemy, pathfinder, navigationState);
            var catalystState = new EnemyCatalystState(frontDetector, animIdle);
            var entranceState = new EnemyEntranceState(enemy, pathfinder, walkAnimation, navigationState);

            navigationState.AddSubState(waitState);
            navigationState.AddSubState(followPathState);

            states = new EnemyState[]
            {
                entranceState,
                navigationState,
                waitState,
                followPathState,
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
    }
}
