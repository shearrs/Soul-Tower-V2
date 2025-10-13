using Shears;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Towers
{
    [Serializable]
    public struct TileGroup
    {
        [SerializeField, ReadOnly] private string id;
        [SerializeField] private bool isSpecialGroup;
        [SerializeField, ShowIf("isSpecialGroup")] private SpecialTileGroup specialGroup;
        [SerializeField, ShowIf("!isSpecialGroup")] private Tile tile;
        [SerializeField, ShowIf("!isSpecialGroup"), Range(1, 32)] private int count;

        public readonly string ID => id;

        public readonly int GetCount(GameObject gameObject)
        {
            if (!isSpecialGroup)
                return count;
            else
            {
                var groups = gameObject.GetComponentsInChildren<SpecialTileGroup>();

                foreach (var group in groups)
                {
                    if (group.GroupID == id)
                        return group.Count;   
                }

                return 0;
            }
        }

        public readonly override bool Equals(object obj)
        {
            return obj is TileGroup group &&
                   id == group.id &&
                   EqualityComparer<Tile>.Default.Equals(tile, group.tile) &&
                   count == group.count;
        }

        public readonly override int GetHashCode()
        {
            return HashCode.Combine(id, tile, count);
        }

        public static bool operator==(TileGroup a, TileGroup b)
        {
            return a.id == b.id;
        }

        public static bool operator!=(TileGroup a, TileGroup b)
        {
            return !(a == b);
        }
    }
}
