using Shears;
using Shears.Cameras;
using Shears.Input;
using UnityEngine;

namespace SoulTower.Players
{
    public class ScrollCameraState : CameraState
    {
        [SerializeField] private Range<float> movementRange;
        [SerializeField, Min(0.1f)] private float sensitivity = 1.0f;
        [SerializeField, Range(0f, 1f)] private float drag = 0.1f;

        private IManagedInput moveInput;
        private float velocity;

        public override void Initialize()
        {
            moveInput = InputProvider.GetInput("Move Camera");
        }

        protected override void OnEnter()
        {
            moveInput.Performed += OnMoveInput;
        }

        protected override void OnExit()
        {
            moveInput.Performed -= OnMoveInput;
        }

        protected override void OnLateUpdate()
        {
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            Vector3 currentPos = CameraTransform.position;
            currentPos.y = movementRange.Clamp(currentPos.y + (velocity * Time.deltaTime));

            CameraTransform.position = currentPos;

            velocity += (-velocity * drag);

            if (Mathf.Abs(velocity) < 0.001f)
                velocity = 0f;
        }

        private void OnMoveInput(ManagedInputInfo info)
        {
            float inputValue = moveInput.ReadValue<Vector2>().y;

            velocity += sensitivity * inputValue;

            float currentHeight = CameraTransform.position.y;

            if (currentHeight == movementRange.Min && velocity < 0f || currentHeight == movementRange.Max && velocity > 0f)
                velocity = 0f;
        }
    }
}
