using Shears;
using Shears.Logging;
using Shears.Pathfinding;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Towers
{
    // adding a room:
    // add to the list at the index
    // move all rooms past it up by the height of the room
    // 

    public class Tower : SHMonoBehaviourLogger
    {
        [Header("Tower")]
        [SerializeField] private PathGrid leftEntranceGrid;
        [SerializeField] private PathGrid rightEntranceGrid;
        [SerializeField] private List<Room> rooms = new();

        public PathGrid LeftEntranceGrid => leftEntranceGrid;

        public PathGrid RightEntranceGrid => rightEntranceGrid;

        public event Action RoomsChanged;

        public void AddRoom(Room roomPrefab, int roomIndex = 1)
        {
            if (roomIndex == 0)
            {
                Log("You cannot replace the entry room!", SHLogLevels.Error);
                return;
            }

            var room = Instantiate(roomPrefab);

            rooms.Insert(roomIndex, room);
            float height = 0.0f;

            foreach (var towerRoom in rooms)
            {
                towerRoom.transform.localPosition = Vector3.zero.With(y: height);
                towerRoom.UpdateGrid();

                height += towerRoom.GetHeight();
            }

            RoomsChanged?.Invoke();
        }

        public Room GetEntryRoom()
        {
            return rooms[0];
        }

        public Room GetTopRoom()
        {
            return rooms[^1];
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
    
        public bool HasLeftOpening()
        {
            foreach (var room in rooms)
            {
                if (room.HasLeftOpening)
                    return true;
            }

            return false;
        }

        public bool HasRightOpening()
        {
            foreach (var room in rooms)
            {
                if (room.HasRightOpening)
                    return true;
            }

            return false;
        }
    }
}
