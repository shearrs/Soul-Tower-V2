using System;
using UnityEngine;

namespace SoulTower.Traps
{
    public class Trap : MonoBehaviour
    {
        [SerializeField] private bool isPassive = true;
        [SerializeField, Range(1, 3)] private int size = 1;
        [SerializeField] private TrapPlacementType placementType;

        public int Size => size;
        public TrapPlacementType PlacementType => placementType;

        public event Action Activated;

        public void Activate()
        {
            if (!isPassive)
                return;

            Activated?.Invoke();
        }
    }
}
