using Shears;
using Shears.Detection;
using Shears.StateMachineGraphs;
using UnityEngine;

namespace SoulTower.Enemies
{
    [RequireComponent(typeof(Enemy))]
    public class Bandit : MonoBehaviour
    {
        private const int MAX_FEINT_COUNT = 4;
        private const float FEINT_COOLDOWN = 5.0f;

        [Header("Components")]
        [SerializeField] private StateMachine stateMachine;
        [SerializeField] private EnemyPathfinder pathfinder;
        [SerializeField] private AreaDetector3D frontDetector;
        [SerializeField] private AreaDetector3D bodyDetector;

        [Header("Animations")]
        [SerializeField] private AnimationClip animIdle;
        [SerializeField] private AnimationClip animWalk;

        private readonly Timer feintCooldownTimer = new(FEINT_COOLDOWN);
        private Enemy enemy;
        private EnemyState[] states;
        private int feintCount;

        public bool CanFeint => feintCount < MAX_FEINT_COUNT;

        private void Awake()
        {
            enemy = GetComponent<Enemy>();

            var animIdle = new FixedSpeedAnimation(this.animIdle);
            var animWalk = new MoveSpeedAnimation(enemy, this.animWalk);
            
            var waitState = new EnemyWaitState(animIdle);
            var followPathState = new EnemyFollowPathState(enemy, animWalk);
            var navigationState = new BanditNavigationState(this, frontDetector, bodyDetector);
            var entranceState = new EnemyEntranceState(enemy, animWalk, navigationState);
            var stairsState = new EnemyStairsState(enemy, animWalk, navigationState);
            var attackState = new EnemyAttackState(enemy, animIdle, animWalk, navigationState);
            var catalystState = new EnemyCatalystState(enemy, animWalk, navigationState, attackState);

            var feintState = new BanditFeintState(enemy, this, frontDetector, animWalk, animIdle);
            var dodgeRollState = new BanditDodgeRollState(enemy, frontDetector, animWalk);
            var rushState = new BanditRushState(enemy, bodyDetector);

            navigationState.AddSubState(waitState);
            navigationState.AddSubState(followPathState);
            navigationState.DefaultSubState = followPathState;

            states = new EnemyState[]
            {
                entranceState,
                waitState,
                navigationState,
                followPathState,
                stairsState,
                catalystState,
                attackState,

                feintState,
                dodgeRollState,
                rushState
            };

            foreach (var state in states)
                state.Initialize(enemy, stateMachine, pathfinder);

            stateMachine.AddStates(states);
        }

        private void OnEnable()
        {
            enemy.Spawned += OnSpawned;
            feintCooldownTimer.Completed += ResetFeintCooldown;
        }

        private void OnDisable()
        {
            enemy.Spawned -= OnSpawned;
            feintCooldownTimer.Completed -= ResetFeintCooldown;
        }

        public void IncrementFeintCount()
        {
            if (feintCount == MAX_FEINT_COUNT)
                return;

            feintCount++;

            if (feintCount == MAX_FEINT_COUNT)
                feintCooldownTimer.Start();
        }

        private void ResetFeintCooldown()
        {
            feintCount = 0;
        }

        private void OnSpawned()
        {
            stateMachine.EnterStateOfType<EnemyEntranceState>();
        }
    }
}
