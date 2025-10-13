using Shears;
using UnityEngine;

namespace SoulTower.Towers
{
    [SelectionBase]
    public class Tile : MonoBehaviour
    {
        public static readonly float TILE_OFFSET = 1.0f;

        [SerializeField] private TileType type = TileType.Floor;

        public TileType Type => type;
    }
}
