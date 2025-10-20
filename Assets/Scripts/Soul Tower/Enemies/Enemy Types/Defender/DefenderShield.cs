using Shears;
using Shears.Signals;
using Shears.Tweens;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Enemies
{
    [RequireComponent(typeof(DefenderShieldHitReceiver))]
    public class DefenderShield : MonoBehaviour
    {
        private static readonly Direction[] DIRECTIONS = { Direction.Right, Direction.Up, Direction.Left };

        public static bool Locked { get; set; } = false;

        public enum Direction { Right, Up, Left }
        
        #region Variables
        [Header("Transforms")]
        [SerializeField] private Transform pivot;
        [SerializeField] private Transform collisionParent;

        [Header("Collision Positions")]
        [SerializeField, Min(0.01f)] private float xShieldDistance;
        [SerializeField, Min(0.01f)] private float yShieldDistance;

        [FoldoutGroup("Bones", 12)]
        [Header("Bones")]
        [SerializeField] private Transform upperArm;
        [SerializeField] private Transform lowerArm;
        [SerializeField] private Transform torso;

        [Header("Upper Arm")]
        [SerializeField] private Vector3 upperLeftRotation;
        [SerializeField] private Vector3 upperUpRotation;
        [SerializeField] private Vector3 upperRightRotation;

        [Header("Lower Arm")]
        [SerializeField] private Vector3 lowerLeftRotation;
        [SerializeField] private Vector3 lowerUpRotation;
        [SerializeField] private Vector3 lowerRightRotation;

        [Header("Torso")]
        [SerializeField] private Vector3 torsoRightRotation;
        [SerializeField] private Vector3 torsoUpRotation;
        [SerializeField] private Vector3 torsoLeftRotation;

        private readonly List<Direction> possibleDirections = new();
        private readonly Timer tweenTimer = new(0.6f);
        private DefenderShieldHitReceiver hitReceiver;
        private Direction currentDirection;
        private bool firstDirection = true;

        private Vector3 nextCollisionDirection;
        private float nextCollisionDistance;
        private Vector3 collisionDirection;
        private float collisionDistance;

        private Quaternion upperArmRotation;
        private Quaternion lowerArmRotation;
        private Quaternion torsoRotation;
        private Quaternion upperPreviousRotation;
        private Quaternion lowerPreviousRotation;
        private Quaternion torsoPreviousRotation;

        public event Action HitReceived;
        #endregion

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ResetLocked()
        {
            Locked = false;
        }
        
        // first direction should probably be performed by the defender
        private void Awake()
        {
            hitReceiver = GetComponent<DefenderShieldHitReceiver>();

            RandomizeDirection();
        }

        private void OnEnable()
        {
            SignalShuttle.Register<SetDefenderShieldsSignal>(OnSetSignal);
            hitReceiver.HitBlocked += OnHitBlocked;
        }

        private void OnDisable()
        {
            SignalShuttle.Deregister<SetDefenderShieldsSignal>(OnSetSignal);
            hitReceiver.HitBlocked -= OnHitBlocked;
        }

        private void LateUpdate()
        {
            if (collisionParent == null || pivot == null)
                return;

            if (!tweenTimer.IsDone)
            {
                upperArm.localRotation = Quaternion.Slerp(upperPreviousRotation, upperArmRotation, tweenTimer.Percentage);
                lowerArm.localRotation = Quaternion.Slerp(lowerPreviousRotation, lowerArmRotation, tweenTimer.Percentage);
                torso.localRotation = Quaternion.Slerp(torsoPreviousRotation, torsoRotation, tweenTimer.Percentage);

                if (collisionParent.gameObject.activeSelf)
                    collisionParent.gameObject.SetActive(false);

                return;
            }
            else
            {
                upperArm.localRotation = upperArmRotation;
                lowerArm.localRotation = lowerArmRotation;
                torso.localRotation = torsoRotation;

                collisionDirection = nextCollisionDirection;
                collisionDistance = nextCollisionDistance;

                if (!collisionParent.gameObject.activeSelf)
                    collisionParent.gameObject.SetActive(true);
            }

            if (collisionDirection == Vector3.zero)
            {
                collisionDistance = nextCollisionDistance;
                collisionDirection = nextCollisionDirection;
            }

            collisionParent.SetPositionAndRotation
            (
                pivot.position + (collisionDistance * collisionDirection),
                Quaternion.LookRotation(collisionDirection)
            );
        }

        private void OnSetSignal(SetDefenderShieldsSignal signal)
        {
            GoToDirection(signal.Direction);
        }

        private void OnHitBlocked()
        {
            HitReceived?.Invoke();
        }

        public void RandomizeDirection()
        {
            if (Locked)
                return;

            possibleDirections.AddRange(DIRECTIONS);

            if (!firstDirection)
                possibleDirections.Remove(currentDirection);
            else
                firstDirection = false;

            int random = UnityEngine.Random.Range(0, possibleDirections.Count);
            var direction = possibleDirections[random];

            GoToDirection(direction);
        }

        private void GoToDirection(Direction direction)
        {
            Vector3 offsetDirection;
            float offsetMagnitude;

            switch (direction)
            {
                case Direction.Right:
                    offsetDirection = Vector3.right;
                    offsetMagnitude = xShieldDistance;
                    upperArmRotation = Quaternion.Euler(upperRightRotation);
                    lowerArmRotation = Quaternion.Euler(lowerRightRotation);
                    torsoRotation = Quaternion.Euler(torsoRightRotation);
                    break;
                case Direction.Up:
                    offsetDirection = Vector3.up;
                    offsetMagnitude = yShieldDistance;
                    upperArmRotation = Quaternion.Euler(upperUpRotation);
                    lowerArmRotation = Quaternion.Euler(lowerUpRotation);
                    torsoRotation = Quaternion.Euler(torsoUpRotation);
                    break;
                case Direction.Left:
                    offsetDirection = Vector3.left;
                    offsetMagnitude = xShieldDistance;
                    upperArmRotation = Quaternion.Euler(upperLeftRotation);
                    lowerArmRotation = Quaternion.Euler(lowerLeftRotation);
                    torsoRotation = Quaternion.Euler(torsoLeftRotation);
                    break;
                default:
                    offsetDirection = Vector3.right;
                    offsetMagnitude = xShieldDistance;
                    upperArmRotation = Quaternion.Euler(upperRightRotation);
                    lowerArmRotation = Quaternion.Euler(lowerRightRotation);
                    torsoRotation = Quaternion.Euler(torsoRightRotation);
                    break;
            }

            currentDirection = direction;
            nextCollisionDirection = offsetDirection;
            nextCollisionDistance = offsetMagnitude;

            upperPreviousRotation = upperArm.localRotation;
            lowerPreviousRotation = lowerArm.localRotation;
            torsoPreviousRotation = torso.localRotation;
            tweenTimer.Start();
        }

        private void OnDrawGizmosSelected()
        {
            if (pivot == null)
                return;

            Gizmos.color = Color.yellow;

            GizmosUtil.DrawArrow(pivot.position, Vector3.right * xShieldDistance, Vector3.up, headColor: Color.red);
            GizmosUtil.DrawArrow(pivot.position, Vector3.left * xShieldDistance, Vector3.up, headColor: Color.red);
            GizmosUtil.DrawArrow(pivot.position, Vector3.up * yShieldDistance, Vector3.right, headColor: Color.red);
        }
    }
}
