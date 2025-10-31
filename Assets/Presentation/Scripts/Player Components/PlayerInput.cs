using Shears.Input;
using UnityEngine;

namespace Presentation
{
    [DefaultExecutionOrder(-100)]
    public class PlayerInput : MonoBehaviour
    {
        public ManagedInputMap inputMap;

        private IManagedInput moveInput;
        private IManagedInput lookInput;
        private IManagedInput sprintInput;
        private IManagedInput jumpInput;

        public Vector2 MoveInput => moveInput.ReadValue<Vector2>();
        public Vector2 LookInput => lookInput.ReadValue<Vector2>();
        public bool SprintInput => sprintInput.IsPressed();
        public bool JumpInput => jumpInput.WasPressedThisFrame();

        public event ManagedInputEvent JumpPerformed { add => jumpInput.Performed += value; remove => jumpInput.Performed -= value; }

        private void Awake()
        {
            inputMap.GetInputs(
                ("Move", i => moveInput = i),
                ("Look", i => lookInput = i),
                ("Sprint", i => sprintInput = i),
                ("Jump", i => jumpInput = i)
            );
        }
    }
}
