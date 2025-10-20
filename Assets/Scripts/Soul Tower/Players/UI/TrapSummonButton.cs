using Shears.UI;
using SoulTower.Traps;
using System;
using TMPro;
using UnityEngine;

namespace SoulTower.Players.UI
{
    [RequireComponent(typeof(CanvasButton))]
    public class TrapSummonButton : MonoBehaviour
    {
        [SerializeField] private Trap trap;
        [SerializeField] private TextMeshProUGUI cost;

        private CanvasButton button;

        public Trap Trap => trap;

        public event Action<Trap> Clicked;

        private void Awake()
        {
            button = GetComponent<CanvasButton>();
            cost.text = trap.Cost.ToString();
        }

        private void OnEnable()
        {
            button.Clicked += OnClicked;
        }

        private void OnDisable()
        {
            button.Clicked -= OnClicked;
        }

        private void OnClicked() => Clicked?.Invoke(trap);

        public void SetSelectable(bool setActive)
        {
            button.Selectable = setActive;
        }
    }
}
