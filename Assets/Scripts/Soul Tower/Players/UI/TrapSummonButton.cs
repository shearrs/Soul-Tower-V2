using Shears.Logging;
using Shears.Signals;
using Shears.UI;
using SoulTower.Currency;
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

        public event Action<Trap> Clicked;

        private void Awake()
        {
            if (trap == null)
            {
                SHLogger.Log("TrapSummonButton doesn't have a trap assigned!", SHLogLevels.Error);
                return;
            }

            button = GetComponent<CanvasButton>();
            cost.text = trap.Cost.ToString();

            button.Selectable = SoulsManager.SoulPoints >= trap.Cost;
        }

        private void OnEnable()
        {
            button.Clicked += OnClicked;

            SignalShuttle.Register<SoulsChangedSignal>(OnSoulsChanged);
        }

        private void OnDisable()
        {
            button.Clicked -= OnClicked;

            SignalShuttle.Deregister<SoulsChangedSignal>(OnSoulsChanged);
        }

        private void OnClicked() => Clicked?.Invoke(trap);

        private void OnSoulsChanged(SoulsChangedSignal signal)
        {
            button.Selectable = signal.Souls >= trap.Cost;
        }
    }
}
