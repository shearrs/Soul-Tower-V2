using Shears;
using SoulTower.Towers;
using System;
using UnityEngine;

namespace SoulTower.Traps
{
    public class Trap : MonoBehaviour
    {
        [SerializeField] private TrapData data;

        private readonly Timer cooldownTimer = new();
        private bool onCooldown = false;

        public int Size => data.Size;
        public TileType PlacementType => data.PlacementType;

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
            if (data.IsPassive || onCooldown)
                return;

            onCooldown = true;
            cooldownTimer.Start(data.Cooldown);

            Activated?.Invoke(this);
        }

        private void OnTimerCompleted()
        {
            onCooldown = false;

            CooldownCompleted?.Invoke(this);
        }
    }
}
