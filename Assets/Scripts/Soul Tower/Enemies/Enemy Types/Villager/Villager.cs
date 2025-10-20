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

        [Header("Weapons")]
        [SerializeField] private Transform weaponContainer;
        [SerializeField] private EnemyWeapon[] possibleWeapons;

        [Header("Animations")]
        [SerializeField] private AnimationClip animIdle;
        [SerializeField] private AnimationClip animWalk;
        [SerializeField] private AnimationClip animWindup;
        [SerializeField] private AnimationClip animAttack;

        private Enemy enemy;
        private EnemyState[] states;

        private void Awake()
        {
            enemy = GetComponent<Enemy>();

            var animIdle = new FixedSpeedAnimation(this.animIdle);
            var animWalk = new MoveSpeedAnimation(enemy, this.animWalk);
            var animWindup = new FixedSpeedAnimation(this.animWindup);
            var animAttack = new FixedSpeedAnimation(this.animAttack);

            var waitState = new EnemyWaitState(animIdle);
            var followPathState = new EnemyFollowPathState(enemy, animWalk);
            var navigationState = new EnemyNavigationState(frontDetector, bodyDetector, waitState, followPathState);
            var entranceState = new EnemyEntranceState(enemy, animWalk, navigationState);
            var stairsState = new EnemyStairsState(enemy, animWalk, navigationState);
            var attackState = new EnemyAttackState(enemy, animWindup, animAttack, animIdle, navigationState);
            var catalystState = new EnemyCatalystState(enemy, animWalk, navigationState, attackState);

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
                attackState
            };

            foreach (var state in states)
                state.Initialize(enemy, stateMachine, pathfinder);

            stateMachine.AddStates(states);

            CreateWeapon();
        }

        private void OnEnable()
        {
            enemy.Spawned += OnSpawned;
        }

        private void OnDisable()
        {
            enemy.Spawned -= OnSpawned;
        }

        private void CreateWeapon()
        {
            EnemyWeapon weaponPrefab;
            int roll = Random.Range(0, 20);

            if (roll < 10)
                weaponPrefab = possibleWeapons[0];
            else if (roll < 18)
                weaponPrefab = possibleWeapons[1];
            else
                weaponPrefab = possibleWeapons[2];

            var weapon = Instantiate(weaponPrefab);

            weapon.transform.SetParent(weaponContainer);
            weapon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        private void OnSpawned()
        {
            stateMachine.EnterStateOfType<EnemyEntranceState>();
        }
    }
}
