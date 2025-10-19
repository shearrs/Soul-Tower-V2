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

        [Header("Animations")]
        [SerializeField] private AnimationClip animIdle;
        [SerializeField] private AnimationClip animWalk;

        private Enemy enemy;
        private EnemyState[] states;

        private void Awake()
        {
            enemy = GetComponent<Enemy>();

            var animIdle = new FixedSpeedAnimation(this.animIdle);
            var animWalk = new MoveSpeedAnimation(enemy, this.animWalk);

            var followPathState = new EnemyFollowPathState(enemy, animWalk);
            var entranceState = new EnemyEntranceState(enemy, animWalk, followPathState);
            var stairsState = new EnemyStairsState(enemy, animWalk, followPathState);
            var attackState = new EnemyAttackState(enemy, animIdle, animWalk, followPathState);
            var catalystState = new EnemyCatalystState(enemy, animWalk, followPathState, attackState);

            states = new EnemyState[]
            {
                entranceState,
                followPathState,
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
    }
}
