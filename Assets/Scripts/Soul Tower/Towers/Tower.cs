using Shears;
using Shears.Logging;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Towers
{
    public class Tower : SHMonoBehaviourLogger
    {
        [SerializeField] private List<Room> rooms = new();

        public Room GetEntryRoom()
        {
            return rooms[0];
        }

        public bool IsEntryRoom(Room room)
        {
            if (rooms.Count == 0)
                return false;
            else
                return room == rooms[0];
        }

        public bool IsTopRoom(Room room)
        {
            if (rooms.Count == 0)
                return false;
            else
                return room == rooms[^1];
        }

        public Room GetNextRoom(Room currentRoom)
        {
            if (currentRoom == null)
                return GetEntryRoom();

            int index = rooms.IndexOf(currentRoom);

            if (index == -1)
            {
                SHLogger.Log("Tower does not contain room: " + currentRoom.name, SHLogLevels.Error);
                
                return null;
            }
            else if (index == rooms.Count - 1)
            {
                SHLogger.Log($"Cannot get next room, current room {currentRoom.name} is the last room of the tower!", SHLogLevels.Error);

                return null;
            }

            return rooms[index + 1];
        }

        public Room GetPreviousRoom(Room currentRoom)
        {
            if (currentRoom == null)
                return GetEntryRoom();

            int index = rooms.IndexOf(currentRoom);

            if (index == -1)
            {
                SHLogger.Log("Tower does not contain room: " + currentRoom.name, SHLogLevels.Error);

                return null;
            }
            else if (index == 0)
            {
                SHLogger.Log($"Cannot get previous room, current room {currentRoom.name} is the first room of the tower!", SHLogLevels.Error);

                return null;
            }

            return rooms[index - 1];
        }

        public Room GetRoomForPosition(Vector3 worldPosition)
        {
            if (rooms.Count == 0)
            {
                Log("Tower has no rooms!", SHLogLevels.Error);
                return null;
            }

            worldPosition.z = 0;

            Room closestRoom = rooms[0];
            Vector3 roomPosition = closestRoom.Center.XY();
            float closestSqrDistance = (worldPosition - roomPosition).sqrMagnitude;

            for (int i = 1; i < rooms.Count; i++)
            {
                var room = rooms[i];
                roomPosition = room.Center.XY();

                float sqrDistance = (worldPosition - roomPosition).sqrMagnitude;

                if (sqrDistance < closestSqrDistance)
                {
                    closestRoom = room;
                    closestSqrDistance = sqrDistance;
                }
            }

            return closestRoom;
        }
    }
}
