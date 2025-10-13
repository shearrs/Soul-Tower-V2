using UnityEngine;

namespace SoulTower.Towers
{
    public class TileSubgroup : MonoBehaviour
    {
        [SerializeField] private Tile tile;
        [SerializeField, Range(1, 32)] private int count = 1;
        [SerializeField] protected TileType tileType = TileType.Floor;
        [SerializeField] private Vector3 rotation = Vector3.zero;

        public int Count => count;
    }
}
