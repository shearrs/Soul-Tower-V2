using UnityEngine;

namespace SoulTower.Towers
{
    public class TileSubgroup : MonoBehaviour
    {
        [SerializeField] private Tile tile;
        [SerializeField, Range(1, 32)] private int count;
    }
}
