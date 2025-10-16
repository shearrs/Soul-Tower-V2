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
        [SerializeField] private AnimationClip animIdle;
        [SerializeField] private AnimationClip animWalk;

        private EnemyState[] states;
        private Enemy enemy;

        private void Awake()
        {
            enemy = GetComponent<Enemy>();

            EnemyState waitState = new EnemyWaitState(animIdle);
            EnemyState followPathState = new EnemyFollowPathState(enemy, pathfinder, animWalk);
            EnemyState navigationState = new EnemyNavigationState(frontDetector, bodyDetector, waitState, followPathState);

            states = new EnemyState[]
            {
                navigationState,
                waitState,
                followPathState,
                new EnemyStairsState(enemy.Tower, enemy, pathfinder, navigationState),
                new EnemyCatalystState(frontDetector)
            };

            navigationState.AddSubState(waitState);
            navigationState.AddSubState(followPathState);

            foreach (var state in states)
                state.Initialize(enemy, stateMachine);

            stateMachine.AddStates(states);
        }

        private void Start()
        {
            stateMachine.EnterStateOfType<EnemyNavigationState>();
        }
    }
}
