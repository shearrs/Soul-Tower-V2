using Shears;
using Shears.Signals;
using SoulTower.HitDetection;
using SoulTower.Towers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Enemies
{
    [SelectionBase]
    public class Enemy : MonoBehaviour, IPathEntity
    {
        #region Fields
        private static readonly Vector3 HEIGHT_OFFSET = 0.5f * Vector3.down;
        private static readonly float PATH_UPDATE_RATE = 0.5f;

        [Header("Tower")]
        [SerializeField] private Tower tower;
        [SerializeField, ReadOnly] private Room currentRoom;

        [Header("Components")]
        [SerializeField] private EnemyModel model;

        [Header("Data")]
        [SerializeField] private EnemyData data;
        [SerializeField] private EnemyStatusFlags statusFlags;
        [SerializeField] private float moveSpeed;

        private readonly HashSet<DamageType> immuneDamageTypes = new();
        private float baseMoveSpeed;
        private float moveSpeedPercentage = 1.0f; // set by slows
        #endregion

        #region Properties
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
        public Catalyst.AttackPoint TargetAttackPoint { get; internal set; }
        public float PathUpdateRate => PATH_UPDATE_RATE;
        public EnemyModel Model => model;
        public EnemySpawnFlags SpawnFlags => data.SpawnFlags;
        public EnemyStatusFlags StatusFlags => statusFlags;
        public float BaseMoveSpeed => baseMoveSpeed;
        public float ResolvedMoveSpeed => moveSpeedPercentage * moveSpeed;
        public float RotationSpeed => model.RotationSpeed * moveSpeed;
        #endregion

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
            
            foreach (var type in data.ImmuneDamageTypes)
            {
                if (!immuneDamageTypes.Contains(type))
                    immuneDamageTypes.Add(type);
            }
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
   
        internal void SetMoveSpeedPercentage(float t)
        {
            moveSpeedPercentage = t;
        }

        public Vector3 GetNodePosition(Vector3 nodePosition)
        {
            return nodePosition + HEIGHT_OFFSET;
        }

        public bool IsAtNodePosition(Vector3 position)
        {
            return transform.position == position + HEIGHT_OFFSET;
        }
    
        public bool CanTakeDamageFrom(DamageData data)
        {
            return !immuneDamageTypes.Contains(data.Type);
        }
    
        public void Damage(int amount)
        {
            if (amount > 0)
            {
                SignalShuttle.Emit(new EnemyDiedSignal(this));
                Destroy(gameObject);
            }
        }
    }
}
