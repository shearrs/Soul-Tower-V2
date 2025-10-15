using Shears.Logging;
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
            if (currentRoom == null)
                return GetEntryRoom();

            int index = rooms.IndexOf(currentRoom);

            if (index == -1 || index == rooms.Count - 1)
            {
                SHLogger.Log("Tower does not contain room: " + currentRoom.name);
                
                return null;
            }

            return rooms[index + 1];
        }
    }
}
