using Shears;
using Shears.Pathfinding;
using SoulTower.Towers;
using System;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class Enemy : MonoBehaviour, IPathEntity
    {
        private static readonly Vector3 HEIGHT_OFFSET = 0.5f * Vector3.down;
        private static readonly float PATH_UPDATE_RATE = 0.5f;

        [Header("Tower")]
        [SerializeField] private Tower tower;
        [SerializeField, ReadOnly] private Room currentRoom;

        [Header("Components")]
        [SerializeField] private EnemyModel model;

        [Header("Data")]
        [SerializeField] private EnemyData data;
        [SerializeField] private float moveSpeed;

        private float baseMoveSpeed;

        public Tower Tower => tower;
        public Room CurrentRoom
        {
            get => currentRoom;
            set
            {
                if (currentRoom == value)
                    return;

                currentRoom = value;
                RoomChanged?.Invoke(value);
            }
        }
        public float PathUpdateRate => PATH_UPDATE_RATE;
        public EnemyModel Model => model;
        public EnemySpawnFlags SpawnFlags => data.SpawnFlags;
        public float BaseMoveSpeed => baseMoveSpeed;
        public float MoveSpeed => moveSpeed;

        public event Action Spawned;
        public event Action<Room> RoomChanged;

        private void OnValidate()
        {
            if (Application.isPlaying)
                return;

            Invoke(nameof(SetLayer), 0.0f);
        }

        private void Awake()
        {
            baseMoveSpeed = data.MoveSpeedRange.Random();
            moveSpeed = baseMoveSpeed;
        }

        private void SetLayer()
        {
            int layer = LayerMask.NameToLayer("Enemy");
            gameObject.layer = layer;

            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                var child = gameObject.transform.GetChild(i);

                child.gameObject.layer = layer;
            }
        }

        public void Spawn(Tower tower)
        {
            this.tower = tower;

            Spawned?.Invoke();
        }

        public void SetMoveSpeed(float speed, bool goAboveBaseSpeed = false)
        {
            if (goAboveBaseSpeed)
                moveSpeed = speed;
            else
                moveSpeed = Mathf.Clamp(speed, 0, baseMoveSpeed);
        }
   
        public Vector3 GetNodePosition(Vector3 nodePosition)
        {
            return nodePosition + HEIGHT_OFFSET;
        }

        public bool IsAtNodePosition(Vector3 position)
        {
            return transform.position == position + HEIGHT_OFFSET;
        }
    }
}
