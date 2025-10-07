using System;
using UnityEngine;

namespace SoulTower.Traps
{
    public class Trap : MonoBehaviour
    {
        [SerializeField] private bool canBeActivated = true;

        public event Action Activated;

        public void Activate()
        {
            if (!canBeActivated)
                return;

            Activated?.Invoke();
        }
    }
}
