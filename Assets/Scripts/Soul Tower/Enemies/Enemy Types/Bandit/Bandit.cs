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
        private const float STAIRS_FEINT_COOLDOWN = 0.5f;

        [Header("Components")]
        [SerializeField] private StateMachine stateMachine;
        [SerializeField] private EnemyPathfinder pathfinder;
        [SerializeField] private AreaDetector3D frontDetector;
        [SerializeField] private AreaDetector3D bodyDetector;

        [Header("Animations")]
        [SerializeField] private AnimationClip animIdle;
        [SerializeField] private AnimationClip animWalk;
        [SerializeField] private AnimationClip animJump;
        [SerializeField] private AnimationClip animDodge;
        [SerializeField] private AnimationClip animLand;

        private readonly Timer feintCooldownTimer = new(FEINT_COOLDOWN);
        private readonly Timer stairsCooldown = new(STAIRS_FEINT_COOLDOWN);
        private Enemy enemy;
        private EnemyState[] states;
        private int feintCount;

        public bool CanFeint => feintCount < MAX_FEINT_COUNT && stairsCooldown.IsDone;

        private void Awake()
        {
            enemy = GetComponent<Enemy>();

            var idleAnim = new FixedSpeedAnimation(animIdle);
            var walkAnim = new MoveSpeedAnimation(enemy, animWalk);
            var jumpAnim = new FixedSpeedAnimation(animJump);
            var dodgeAnim = new FixedSpeedAnimation(animDodge);
            var landAnim = new FixedSpeedAnimation(animLand);

            var waitState = new EnemyWaitState(idleAnim);
            var followPathState = new EnemyFollowPathState(enemy, walkAnim);
            var navigationState = new BanditNavigationState(this, frontDetector, bodyDetector);
            var entranceState = new EnemyEntranceState(enemy, walkAnim, navigationState);
            var stairsState = new EnemyStairsState(enemy, walkAnim, navigationState);
            var stairsSignalState = new BanditStairsState(this);
            var attackState = new EnemyAttackState(enemy, idleAnim, walkAnim, navigationState);
            var catalystState = new EnemyCatalystState(enemy, walkAnim, navigationState, attackState);

            var feintState = new BanditFeintState(enemy, this, frontDetector, bodyDetector, walkAnim, idleAnim);
            var dodgeRollState = new BanditDodgeRollState(enemy, frontDetector, bodyDetector, jumpAnim, dodgeAnim, landAnim, idleAnim);
            var rushState = new BanditRushState(enemy, bodyDetector, walkAnim);
            var backstepState = new BanditBackstepState(enemy, frontDetector, bodyDetector, walkAnim);

            navigationState.AddSubState(waitState);
            navigationState.AddSubState(followPathState);
            navigationState.DefaultSubState = followPathState;

            stairsState.AddSubState(stairsSignalState);
            stairsState.DefaultSubState = stairsSignalState;

            states = new EnemyState[]
            {
                entranceState,
                waitState,
                navigationState,
                followPathState,
                stairsState,
                stairsSignalState,
                catalystState,
                attackState,

                feintState,
                dodgeRollState,
                rushState,
                backstepState
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
                BeginFeintCooldown();
        }

        public void BeginFeintCooldown()
        {
            feintCooldownTimer.Start();
        }

        public void BeginStairsTimer()
        {
            stairsCooldown.Restart();
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
