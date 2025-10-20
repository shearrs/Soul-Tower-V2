using Shears.Logging;
using Shears.Signals;
using Shears.UI;
using SoulTower.Currency;
using SoulTower.Towers;
using System;
using TMPro;
using UnityEngine;

namespace SoulTower.Players.UI
{
    public class RoomSummonButton : MonoBehaviour
    {
        [SerializeField] private Room room;
        [SerializeField] private TextMeshProUGUI cost;

        private CanvasButton button;

        public event Action<Room> Clicked;

        private void Awake()
        {
            if (room == null)
            {
                SHLogger.Log("RoomSummonButton doesn't have a room assigned!", SHLogLevels.Error);
                return;
            }

            button = GetComponent<CanvasButton>();
            cost.text = room.Cost.ToString();

            button.Selectable = SoulsManager.SoulPoints >= room.Cost;
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

        private void OnClicked() => Clicked?.Invoke(room);

        private void OnSoulsChanged(SoulsChangedSignal signal)
        {
            button.Selectable = signal.Souls >= room.Cost;
        }
    }
}
