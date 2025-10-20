using SoulTower.Currency;
using SoulTower.Towers;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Players.UI
{
    public class RoomPlacementUI : MonoBehaviour
    {
        [SerializeField] private Player player;

        private readonly List<RoomSummonButton> buttons = new();

        private void Awake()
        {
            GetComponentsInChildren(buttons);
        }

        private void OnEnable()
        {
            foreach (var button in buttons)
                button.Clicked += OnRoomButtonClicked;
        }

        private void OnDisable()
        {
            foreach (var button in buttons)
                button.Clicked -= OnRoomButtonClicked;
        }

        private void OnRoomButtonClicked(Room room)
        {
            SoulsManager.UpdateSouls(-room.Cost);
            player.Tower.AddRoom(room);
        }
    }
}
