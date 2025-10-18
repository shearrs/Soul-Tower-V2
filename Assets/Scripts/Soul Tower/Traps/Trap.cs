using Shears;
using Shears.Logging;
using SoulTower.HitDetection;
using SoulTower.Towers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Traps
{
    public class Trap : MonoBehaviour
    {
        [SerializeField] private TrapData data;

        private readonly Timer cooldownTimer = new();
        private bool onCooldown = false;

        public int Size
        {
            get
            {
                if (data == null)
                    LogMissingDataError();

                return data.Size;
            }
        }

        public TileType PlacementType
        {
            get
            {
                if (data == null)
                    LogMissingDataError();

                return data.PlacementType;
            }
        }

        public bool IsOnCooldown => onCooldown;

        public IReadOnlyCollection<DamageData> DamageData
        {
            get
            {
                if (data == null)
                    LogMissingDataError();

                return data.DamageData;
            }
        }

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

            BeginCooldown();

            Activated?.Invoke(this);
        }

        public void BeginCooldown()
        {
            onCooldown = true;
            cooldownTimer.Start(data.Cooldown);
        }

        public void ResetCooldown()
        {
            cooldownTimer.Stop();
            onCooldown = false;

            CooldownCompleted?.Invoke(this);
        }

        private void OnTimerCompleted()
        {
            ResetCooldown();
        }
    
        private void LogMissingDataError()
        {
            SHLogger.Log("Trap has no data set! You probably need to assign it in the trap prefab's inspector.", SHLogLevels.Error);
        }
    }
}
