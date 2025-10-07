using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Towers
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private List<Tile> tiles = new();
    }
}
