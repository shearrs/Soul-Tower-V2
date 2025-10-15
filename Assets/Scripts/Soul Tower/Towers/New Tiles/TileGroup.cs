using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Towers
{
    [SelectionBase]
    public class TileGroup : MonoBehaviour
    {
        [SerializeField] private List<Tile> defaultTiles = new();
        [SerializeField] private List<TileSubgroup> defaultTileGroups = new();
        [SerializeField] private TileType tileType = TileType.Floor;

        public TileType TileType => tileType;

        public T GetDefaultTile<T>() where T : Component
        {
            foreach (var tile in defaultTiles)
            {
                if (tile != null && tile.TryGetComponent(out T t))
                    return t;
            }

            return null;
        }

        public TileSubgroup GetDefaultTileGroup(Type type)
        {
            foreach (var group in defaultTileGroups)
            {
                if (group.GetType() == type)
                    return group;
            }

            return null;
        }
    }
}
