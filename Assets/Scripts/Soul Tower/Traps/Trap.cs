using Shears;
using System;
using UnityEngine;

namespace SoulTower.Traps
{
    public class Trap : MonoBehaviour
    {
        [Header("Use Settings")]
        [SerializeField] private bool isPassive = false;
        [SerializeField, ShowIf("!isPassive")] private float cooldown = 5f;

        [Header("Placement Settings")]
        [SerializeField, Range(1, 3)] private int size = 1;
        [SerializeField] private TrapPlacementType placementType;

        private readonly Timer cooldownTimer = new();
        private bool onCooldown = false;

        public int Size => size;
        public TrapPlacementType PlacementType => placementType;

        public event Action<Trap> Activated;
        public event Action<Trap> CooldownCompleted;

        private void OnEnable()
        {
            cooldownTimer.Completed += OnTimerCompleted;
        }

        private void OnDisable()
        {
            cooldownTimer.Completed -= OnTimerCompleted;
        }

        public void Activate()
        {
            if (isPassive || onCooldown)
                return;

            onCooldown = true;
            cooldownTimer.Start(cooldown);

            Activated?.Invoke(this);
        }

        private void OnTimerCompleted()
        {
            onCooldown = false;

            CooldownCompleted?.Invoke(this);
        }
    }
}
