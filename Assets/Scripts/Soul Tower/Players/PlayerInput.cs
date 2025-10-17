using Shears.Input;
using Shears.Signals;
using UnityEngine;

namespace SoulTower.Players
{
    [DefaultExecutionOrder(-100)]
    public class PlayerInput : MonoBehaviour
    {
        [SerializeField] private ManagedInputMap cameraInputMap;

        private bool isEnabled = true;

        public IManagedInput MoveCamera { get; private set; }
        public IManagedInput SnapCamera { get; private set; }

        private void Awake()
        {
            cameraInputMap.GetInputs
            (
                ("Move Camera", (i) => MoveCamera = i),
                ("Snap Camera", (i) => SnapCamera = i)
            );

            SignalShuttle.Register<ToggleInputSignal>(OnToggleInputSignal);
        }

        private void OnDestroy()
        {
            SignalShuttle.Deregister<ToggleInputSignal>(OnToggleInputSignal);
        }

        public void Enable()
        {
            if (isEnabled)
                return;

            cameraInputMap.EnableAllInputs();

            isEnabled = true;
        }

        public void Disable()
        {
            if (!isEnabled)
                return;

            cameraInputMap.DisableAllInputs();
            
            isEnabled = false;
        }

        private void OnToggleInputSignal(ToggleInputSignal signal)
        {
            if (signal.EnableInput)
                Enable();
            else
                Disable();
        }
    }
}
