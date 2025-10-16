using Shears;
using Shears.Cameras;
using SoulTower.Towers;
using UnityEngine;

namespace SoulTower.Players
{
    [RequireComponent(typeof(ManagedCamera))]
    public class PlayerCamera : ManagedWrapper<ManagedCamera>
    {
        [SerializeField] private ScrollCameraState scrollState;

        private void Start()
        {
            TypedWrappedValue.AddState(scrollState);
            TypedWrappedValue.SetState(scrollState);
        }

        public void SetTower(Tower tower)
        {
            scrollState.Tower = tower;

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
