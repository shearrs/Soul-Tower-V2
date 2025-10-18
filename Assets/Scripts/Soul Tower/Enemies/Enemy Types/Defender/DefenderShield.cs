using Shears;
using Shears.HitDetection;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Enemies
{
    // might need to just have access to the animator to swap directions, for now lets teleport
    [RequireComponent(typeof(DefenderShieldHitReceiver))]
    public class DefenderShield : MonoBehaviour
    {
        private static readonly Direction[] DIRECTIONS = { Direction.Left, Direction.Up, Direction.Right };

        public enum Direction { Left, Up, Right }

        [SerializeField] private Transform pivot;
        [SerializeField, Min(0.01f)] private float xShieldDistance;
        [SerializeField, Min(0.01f)] private float yShieldDistance;

        private readonly List<Direction> possibleDirections = new();
        private DefenderShieldHitReceiver hitReceiver;
        private Direction currentDirection;
        public event Action HitReceived;

        // first direction should probably be performed by the defender
        private void Awake()
        {
            hitReceiver = GetComponent<DefenderShieldHitReceiver>();

            RandomizeDirection();
        }

        private void OnEnable()
        {
            hitReceiver.HitBlocked += OnHitBlocked;
        }

        private void OnDisable()
        {
            hitReceiver.HitBlocked -= OnHitBlocked;
        }

        private void OnHitBlocked()
        {
            HitReceived?.Invoke();
        }

        public void RandomizeDirection()
        {
            possibleDirections.AddRange(DIRECTIONS);
            possibleDirections.Remove(currentDirection);
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
                case Direction.Left:
                    offsetDirection = Vector3.left;
                    offsetMagnitude = xShieldDistance;
                    break;
                case Direction.Right:
                    offsetDirection = Vector3.right;
                    offsetMagnitude = xShieldDistance;
                    break;
                case Direction.Up:
                    offsetDirection = Vector3.up;
                    offsetMagnitude = yShieldDistance;
                    break;
                default:
                    offsetDirection = Vector3.left;
                    offsetMagnitude = xShieldDistance;
                    break;
            }

            currentDirection = direction;
            transform.SetPositionAndRotation
            (
                pivot.position + (offsetMagnitude * offsetDirection), 
                Quaternion.LookRotation(offsetDirection)
            );
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
