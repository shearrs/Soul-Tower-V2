using Shears;
using UnityEngine;

namespace Presentation
{
    public class PlayerMovement : MonoBehaviour
    {
        public Transform relativeTransform;
        public CharacterController controller;

        public void Move(Vector2 input, float speed)
        {
            var forward = relativeTransform.forward;
            var right = relativeTransform.right;
            var movement = ((input.y * forward) + (input.x * right)).With(y: 0).normalized;

            movement = Time.deltaTime * speed * movement;
            controller.Move(movement);
        }
    }
}
