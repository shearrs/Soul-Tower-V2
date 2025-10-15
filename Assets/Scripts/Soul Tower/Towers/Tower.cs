using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Towers
{
    public class Tower : MonoBehaviour
    {
        [SerializeField] private List<Room> rooms = new();

        public Room GetEntryRoom()
        {
            return rooms[0];
        }

        public Room GetNextRoom(Room currentRoom)
        {
            int index = rooms.IndexOf(currentRoom);

            if (index == -1 || index == rooms.Count - 1)
                return null;

            return rooms[index + 1];
        }
    }
}
