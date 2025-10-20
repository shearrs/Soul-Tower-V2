using Shears;
using Shears.Cameras;
using SoulTower.Towers;
using UnityEngine;

namespace SoulTower.Players
{
    [RequireComponent(typeof(ManagedCamera))]
    public class PlayerCamera : ManagedWrapper<ManagedCamera>
    {
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private ScrollCameraState scrollState;

        private Tower tower;

        private void Start()
        {
            scrollState.ScrollInput = playerInput.MoveCamera;
            scrollState.SnapInput = playerInput.SnapCamera;

            TypedWrappedValue.AddState(scrollState);
            TypedWrappedValue.SetState(scrollState);
        }

        public void SetTower(Tower tower)
        {
            this.tower = tower;
            scrollState.Tower = tower;

            UpdateMaxScrollHeight();
        }

        public void UpdateMaxScrollHeight()
        {
            SetMaxScrollHeight(tower.GetTopRoom().Center.y);
        }

        public void SetMaxScrollHeight(float height)
        {
            const float MIN_HEIGHT = 4.0f;

            height = Mathf.Max(height, MIN_HEIGHT);

            scrollState.ScrollRange = new(MIN_HEIGHT, height);
        }
    }
}
