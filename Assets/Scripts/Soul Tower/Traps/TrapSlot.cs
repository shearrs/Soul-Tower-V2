using Shears;
using Shears.Logging;
using SoulTower.Towers;
using System;
using UnityEngine;

namespace SoulTower.Traps
{
    [RequireComponent(typeof(Tile))]
    public class TrapSlot : SHMonoBehaviourLogger
    {
        [Header("Trap Slot")]
        [SerializeField, ReadOnly] private Trap trap;
        [SerializeField, ReadOnly] private TrapSlotGroup group;
        [SerializeField] private Transform trapContainer;

        private Tile tile;

        internal Transform TrapContainer => trapContainer;
        internal TrapSlotGroup Group { get => group; set => group = value; }
        public Tile Tile
        {
            get
            {
                if (tile == null)
                    tile = GetComponent<Tile>();

                return tile;
            }
        }
        public Trap Trap => trap;
        public TileType PlacementType => Tile.Type;

        public void PlaceTrap(Trap trap) => group.PlaceTrap(trap, this);
        public bool CanPlaceTrap(Trap trap) => group.CanPlaceTrap(trap, this);

        public Vector3 GetTrapPosition(Trap trap) => group.GetTrapPosition(trap, this);
        public Quaternion GetTrapRotation()
        {
            var rotation = trapContainer.rotation;

            if (trapContainer.up == Vector3.right)
                rotation = Quaternion.Euler(180.0f, 0.0f, 0.0f) * rotation;

            return rotation;
        }

        private void Awake()
        {
            tile = GetComponent<Tile>();
        }

        internal void SetTrap(Trap trap)
        {
            this.trap = trap;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            GizmosUtil.DrawArrow(transform.position, transform.up, transform.right, headColor: Color.magenta);
        }
    }
}
