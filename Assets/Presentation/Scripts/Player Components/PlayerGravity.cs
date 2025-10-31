using UnityEngine;

namespace Presentation
{
    public class PlayerGravity : MonoBehaviour
    {
        public PlayerFlags flags;
        public PlayerMovementData movementData;
        public CharacterController controller;

        private float verticalForce;

        private void Update()
        {
            ApplyGravity();
        }

        private void ApplyGravity()
        {
            if (flags.isJumping)
            {
                verticalForce = 0.0f;
                return;
            }
            else if (flags.isGrounded)
            {
                verticalForce = 0.0f;
                return;
            }

            verticalForce += movementData.gravity * Time.deltaTime;
            controller.Move(verticalForce * Time.deltaTime * Vector3.up);
        }
    }
}
