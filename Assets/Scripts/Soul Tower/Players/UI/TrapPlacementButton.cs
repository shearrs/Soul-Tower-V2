using Shears.UI;
using SoulTower.Traps;
using System;
using TMPro;
using UnityEngine;

namespace SoulTower.Players.UI
{
    [RequireComponent(typeof(ManagedUIElement))]
    public class TrapPlacementButton : MonoBehaviour
    {
        [SerializeField] private Trap trap;
        [SerializeField] private TextMeshProUGUI cost;
        [SerializeField] private ManagedImage background;

        private ManagedUIElement button;
        private bool buttonIsActive = true;
        public Trap Trap => trap;

        public event Action<Trap> Clicked;

        private void Awake()
        {
            button = GetComponent<ManagedUIElement>();
            cost.text = trap.Cost.ToString();
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

        public void ChangeButtonActive(bool setActive)
        {
            if(setActive != buttonIsActive)
            {
                buttonIsActive = setActive;
                button.Selectable = setActive;
                button.Hoverable = setActive;
                if (setActive)
                {
                    background.Modulate = Color.white;
                } else
                {
                    background.Modulate = Color.darkGray;
                }
            }
        }
    }
}
