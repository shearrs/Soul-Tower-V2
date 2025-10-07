using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Towers
{
    public class Tower : MonoBehaviour
    {
        [SerializeField] private List<Room> rooms = new();
    }
}
