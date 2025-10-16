using Shears;
using Shears.Cameras;
using Shears.Input;
using Shears.Logging;
using Shears.Tweens;
using SoulTower.Towers;
using UnityEngine;

namespace SoulTower.Players
{
    public class ScrollCameraState : CameraState
    {
        [Header("Scroll State")]
        [SerializeField] private Range<float> movementRange;
        [SerializeField, Min(0.1f)] private float sensitivity = 14.0f;
        [SerializeField, Range(0f, 1f)] private float drag = 0.08f;
        [SerializeField] private TweenData snapTweenData;

        private IManagedInput scrollInput;
        private IManagedInput snapInput;
        private float velocity;
        private Tween snapTween;
        private bool isSnapping = false;

        public Tower Tower { get; set; }
        public Range<float> ScrollRange { get => movementRange; set => movementRange = value; }

        public override void Initialize()
        {
            scrollInput = InputProvider.GetInput("Move Camera");
            snapInput = InputProvider.GetInput("Snap Camera");
        }

        protected override void OnEnter()
        {
            scrollInput.Performed += OnMoveInput;
            snapInput.Performed += OnSnapInput;
        }

        protected override void OnExit()
        {
            scrollInput.Performed -= OnMoveInput;
            snapInput.Performed -= OnSnapInput;
        }

        protected override void OnLateUpdate()
        {
            if (!isSnapping)
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
            if (isSnapping)
            {
                isSnapping = false;
                snapTween.Dispose();
            }

            float inputValue = scrollInput.ReadValue<Vector2>().y;

            velocity += sensitivity * inputValue;

            float currentHeight = CameraTransform.position.y;

            if (currentHeight == movementRange.Min && velocity < 0f || currentHeight == movementRange.Max && velocity > 0f)
                velocity = 0f;
        }

        private void OnSnapInput(ManagedInputInfo info)
        {
            if (Tower == null)
            {
                Log("Camera has no tower assigned!", SHLogLevels.Error);
                return;
            }

            isSnapping = true;
            snapTween.Dispose();

            float input = snapInput.ReadValue<float>();
            Room currentRoom = Tower.GetRoomForPosition(CameraTransform.position);
            Room nextRoom = null;

            if (input > 0 && !Tower.IsTopRoom(currentRoom))
                nextRoom = Tower.GetNextRoom(currentRoom);
            else if (input < 0 && !Tower.IsEntryRoom(currentRoom))
                nextRoom = Tower.GetPreviousRoom(currentRoom);

            if (nextRoom == null)
            {
                Log("Cannot snap to next room, currently at the top or bottom.", SHLogLevels.Verbose);
                nextRoom = currentRoom;
            }

            Vector3 targetPos = nextRoom.Center.With(z: CameraTransform.position.z);
            snapTween = CameraTransform.DoMoveTween(targetPos, snapTweenData);
        }
    }
}
