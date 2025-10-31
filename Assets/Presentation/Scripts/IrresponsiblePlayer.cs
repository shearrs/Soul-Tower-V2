using Shears;
using Shears.Input;
using System.Collections;
using UnityEngine;

namespace Presentation
{
    public class IrresponsiblePlayer : MonoBehaviour
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

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            moveInput = inputMap.GetInput("Move");
            lookInput = inputMap.GetInput("Look");
            sprintInput = inputMap.GetInput("Sprint");
            jumpInput = inputMap.GetInput("Jump");
        }

        private void OnEnable()
        {
            jumpInput.Performed += OnJumpInput;
        }

        private void OnDisable()
        {
            jumpInput.Performed -= OnJumpInput;
        }

        private void Update()
        {
            UpdateIsGrounded();
            Move();
            ApplyGravity();
        }

        private void LateUpdate()
        {
            Look();
        }

        private void Move()
        {
            var input = moveInput.ReadValue<Vector2>();

            var forward = cam.transform.forward;
            var right = cam.transform.right;
            var movement = ((input.y * forward) + (input.x * right)).With(y: 0).normalized;

            float moveSpeed = sprintInput.IsPressed() ? sprintSpeed : speed;

            movement = Time.deltaTime * moveSpeed * movement;
            controller.Move(movement);
        }

        private void Look()
        {
            var input = lookInput.ReadValue<Vector2>();
            var camTransform = cam.transform;
            var forward = camTransform.forward;
            forward.y = 0;

            var currentXRotation = Vector3.SignedAngle(forward, camTransform.forward, camTransform.right);
            var currentYRotation = camTransform.rotation.eulerAngles.y;

            var newXRotation = currentXRotation - input.y * cameraSensitivity;
            var newYRotation = currentYRotation + input.x * cameraSensitivity;

            newXRotation = Mathf.Clamp(newXRotation, minXRotation, maxXRotation);

            camTransform.rotation = Quaternion.Euler(newXRotation, newYRotation, 0f);
        }

        private void UpdateIsGrounded()
        {
            isGrounded = Physics.CheckSphere(transform.position + groundDetectionOffset, groundDetectionRadius, groundDetectionLayers);
        }

        private void ApplyGravity()
        {
            if (isJumping)
            {
                verticalForce = 0.0f;
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

        private void OnJumpInput(ManagedInputInfo info)
        {
            if (isJumping || !isGrounded)
                return;

            StartCoroutine(IEJump());
        }

        private IEnumerator IEJump()
        {
            isJumping = true;

            float verticalForce = jumpForce;

            while (verticalForce > 0)
            {
                controller.Move(new Vector3(0, verticalForce * Time.deltaTime, 0));
                verticalForce += gravity * Time.deltaTime;

                yield return null;
            }

            isJumping = false;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + groundDetectionOffset, groundDetectionRadius);
        }
    }
}
