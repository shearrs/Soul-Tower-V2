using Shears;
using Shears.Pathfinding;
using SoulTower.Towers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class Enemy : MonoBehaviour, IPathEntity
    {
        [SerializeField] private EnemyMovement movement;

        private void Start()
        {
            movement.Enable();
        }
    }
}
