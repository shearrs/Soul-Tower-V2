using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Towers
{
    [SelectionBase]
    public class TileGroup : MonoBehaviour
    {
        [SerializeField] private List<Tile> defaultTiles = new();

        public T GetDefaultTile<T>() where T : Component
        {
            foreach (var tile in defaultTiles)
            {
                if (tile != null && tile.TryGetComponent(out T t))
                    return t;
            }

            return null;
        }
    }
}
