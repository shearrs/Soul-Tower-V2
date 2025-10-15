using Shears.Detection;
using Shears.StateMachineGraphs;
using UnityEngine;

namespace SoulTower.Enemies
{
    [RequireComponent(typeof(Enemy))]
    public class Villager : MonoBehaviour
    {
        [SerializeField] private StateMachine stateMachine;
        [SerializeField] private EnemyPathfinder pathfinder;
        [SerializeField] private AreaDetector3D frontDetector;
        [SerializeField] private AreaDetector3D bodyDetector;

        private EnemyState[] states;
        private Enemy enemy;

        private void Awake()
        {
            enemy = GetComponent<Enemy>();

            EnemyState navigationState;
            EnemyState waitState;

            states = new EnemyState[]
            {
                navigationState = new VillagerNavigationState(frontDetector, bodyDetector),
                waitState = new EnemyWaitState(),
                new VillagerFollowPathState(enemy, pathfinder),
                new EnemyStairsState(enemy.Tower, enemy, pathfinder, navigationState),
                new EnemyCatalystState(frontDetector)
            };

            navigationState.AddSubState(waitState);

            foreach (var state in states)
                state.Initialize(stateMachine);

            stateMachine.AddStates(states);
        }

        private void Start()
        {
            stateMachine.EnterStateOfType<VillagerNavigationState>();
        }
    }
}
