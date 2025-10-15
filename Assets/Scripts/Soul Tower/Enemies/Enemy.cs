using Shears;
using SoulTower.Towers;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class Enemy : MonoBehaviour, IPathEntity
    {
        private static readonly Vector3 HEIGHT_OFFSET = 0.5f * Vector3.down;

        [Header("References")]
        [SerializeField] private Tower tower;
        [SerializeField, ReadOnly] private Room currentRoom;

        [Header("Data")]
        [SerializeField] private EnemyData data;
        [SerializeField] private float moveSpeed;

        private float baseMoveSpeed;

        public float BaseMoveSpeed => baseMoveSpeed;
        public float MoveSpeed => moveSpeed;
        public Tower Tower => tower;
        public Room CurrentRoom { get => currentRoom; internal set => currentRoom = value; }
        public Vector3 HeightOffset => HEIGHT_OFFSET;

        private void Awake()
        {
            baseMoveSpeed = data.MoveSpeedRange.Random();
            moveSpeed = baseMoveSpeed;
            currentRoom = tower.GetEntryRoom();
        }

        public void SetMoveSpeed(float speed, bool goAboveBaseSpeed = false)
        {
            if (goAboveBaseSpeed)
                moveSpeed = speed;
            else
                moveSpeed = Mathf.Clamp(speed, 0, baseMoveSpeed);
        }
    }
}
