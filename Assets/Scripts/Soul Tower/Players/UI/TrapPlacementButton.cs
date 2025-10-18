using Shears.UI;
using SoulTower.Traps;
using System;
using UnityEngine;

namespace SoulTower.Players.UI
{
    [RequireComponent(typeof(ManagedUIElement))]
    public class TrapPlacementButton : MonoBehaviour
    {
        [SerializeField] private Trap trap;

        private ManagedUIElement button;

        public event Action<Trap> Clicked;

        private void Awake()
        {
            button = GetComponent<ManagedUIElement>();
        }

        private void OnEnable()
        {
            button.ClickEnded += OnClicked;
        }

        private void OnDisable()
        {
            button.ClickEnded -= OnClicked;
        }

        private void OnClicked() => Clicked?.Invoke(trap);
    }
}
