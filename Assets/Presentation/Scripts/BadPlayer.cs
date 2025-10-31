using Shears;
using Shears.Input;
using UnityEngine;

namespace Presentation
{
    public class BadPlayer : MonoBehaviour
    {
        public ManagedInputMap inputMap;
        private IManagedInput moveInput;
        private IManagedInput lookInput;
        private IManagedInput sprintInput;
        private IManagedInput jumpInput;

        public CharacterController controller;
        public float speed = 4.0f;
        public float sprintSpeed = 6.0f;
        public float gravity = -9.81f;
        public float jumpForce = 5.0f;
        private float verticalForce = 0.0f;
        private bool isJumping = false;
        private bool isGrounded = false;

        public Vector3 groundDetectionOffset;
        public float groundDetectionRadius;
        public LayerMask groundDetectionLayers;

        public Camera cam;
        public float cameraSensitivity = 0.25f;
        public float minXRotation = -89.0f;
        public float maxXRotation = 89.0f;

        private void Update()
        {
            // INITIALIZATION
            if (this.moveInput == null)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                this.moveInput = inputMap.GetInput("Move");
                lookInput = inputMap.GetInput("Look");
                sprintInput = inputMap.GetInput("Sprint");
                jumpInput = inputMap.GetInput("Jump");
            }

            isGrounded = Physics.CheckSphere(transform.position + groundDetectionOffset, groundDetectionRadius, groundDetectionLayers);

            // LOOK
            var camInput = lookInput.ReadValue<Vector2>();
            var camTransform = cam.transform;
            var camForward = camTransform.forward;
            camForward.y = 0;

            var currentXRotation = Vector3.SignedAngle(camForward, camTransform.forward, camTransform.right);
            var currentYRotation = camTransform.rotation.eulerAngles.y;

            var newXRotation = currentXRotation - camInput.y * cameraSensitivity;
            var newYRotation = currentYRotation + camInput.x * cameraSensitivity;

            newXRotation = Mathf.Clamp(newXRotation, minXRotation, maxXRotation);

            camTransform.rotation = Quaternion.Euler(newXRotation, newYRotation, 0f);

            // MOVE
            var moveInput = this.moveInput.ReadValue<Vector2>();

            var moveForward = cam.transform.forward;
            var moveRight = cam.transform.right;
            var movement = ((moveInput.y * moveForward) + (moveInput.x * moveRight)).With(y: 0).normalized;

            float moveSpeed = sprintInput.IsPressed() ? sprintSpeed : speed;

            movement = Time.deltaTime * moveSpeed * movement;
            controller.Move(movement);

            // JUMP & GRAVITY
            if (jumpInput.WasPressedThisFrame() && !isJumping && isGrounded)
            {
                isJumping = true;
                verticalForce = jumpForce;
            }

            if (isJumping)
            {
                controller.Move(verticalForce * Time.deltaTime * Vector3.up);
                verticalForce += gravity * Time.deltaTime;

                if (verticalForce <= 0)
                    isJumping = false;

                return;
            }
            else if (isGrounded)
            {
                verticalForce = 0.0f;
                return;
            }

            verticalForce += gravity * Time.deltaTime;
            controller.Move(verticalForce * Time.deltaTime * Vector3.up);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + groundDetectionOffset, groundDetectionRadius);
        }
    }
}
