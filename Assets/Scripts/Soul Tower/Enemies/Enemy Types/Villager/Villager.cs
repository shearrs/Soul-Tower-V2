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

        private VillagerState[] states;
        private Enemy enemy;

        private void Awake()
        {
            enemy = GetComponent<Enemy>();

            VillagerState navigationState;
            VillagerState waitState;

            states = new VillagerState[]
            {
                navigationState = new VillagerNavigationState(frontDetector, bodyDetector),
                waitState = new VillagerWaitState(),
                new VillagerFollowPathState(enemy, pathfinder),
                new VillagerStairsState(enemy.Tower, enemy, pathfinder),
                new VillagerCatalystState(frontDetector)
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
