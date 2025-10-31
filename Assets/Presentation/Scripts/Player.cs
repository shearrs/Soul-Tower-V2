using Shears.Input;
using UnityEngine;

namespace Presentation
{
    public class Player : MonoBehaviour
    {
        public PlayerInput input;
        public PlayerFlags flags;
        public PlayerMovement movement;
        public PlayerJumper jumper;
        public PlayerMovementData movementData;

        private void OnEnable()
        {
            input.JumpPerformed += OnJumpInput;
        }

        private void OnDisable()
        {
            input.JumpPerformed -= OnJumpInput;
        }

        private void Update()
        {
            UpdateMovement();
        }

        private void UpdateMovement()
        {
            float speed = input.SprintInput ? movementData.sprintSpeed : movementData.moveSpeed;

            movement.Move(input.MoveInput, speed);
        }

        private void OnJumpInput(ManagedInputInfo info)
        {
            if (flags.isJumping || !flags.isGrounded)
                return;

            jumper.Jump(movementData.jumpForce, movementData.gravity);
        }
    }
}
