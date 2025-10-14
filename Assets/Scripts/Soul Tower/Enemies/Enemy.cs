using Shears;
using SoulTower.Towers;
using System.Collections;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class Enemy : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private EnemyPathfinder pathfinder;
        [SerializeField] private Tower tower;
        [SerializeField, ReadOnly] private Room currentRoom;

        [Header("Settings")]
        [SerializeField] private float moveSpeed = 1.0f;

        private void Start()
        {
            currentRoom = tower.GetEntryRoom();

            StartCoroutine(IEMove());
        }

        private IEnumerator IEMove()
        {
            while (true)
            {
                if (transform.position == currentRoom.DoorPosition)
                {
                    Debug.Log("next room");
                    currentRoom = tower.GetNextRoom(currentRoom);
                }

                if (currentRoom == null)
                {
                    Debug.Log("break");
                    yield break;
                }

                pathfinder.UpdatePath(transform.position, currentRoom.DoorPosition);
                Vector3 targetPosition = pathfinder.GetTargetPosition();
                Vector3 direction = (targetPosition - transform.position).normalized;

                transform.position += moveSpeed * Time.deltaTime * direction;

                yield return null;
            }
        }
    }
}
