using SoulTower.Towers;
using UnityEngine;

namespace SoulTower.Players
{
    public class PlayerRoomConstructor : MonoBehaviour
    {
        [SerializeField] private Tower tower;
        [SerializeField] private Room room;

        [ContextMenu("Add Room")]
        private void AddRoom()
        {
            tower.AddRoom(room, 1);
        }
    }
}
