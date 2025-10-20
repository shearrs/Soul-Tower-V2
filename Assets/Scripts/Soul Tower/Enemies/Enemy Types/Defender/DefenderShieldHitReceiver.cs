using Shears.HitDetection;
using SoulTower.HitDetection;
using System;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class DefenderShieldHitReceiver : HitReceiver3D, IHitBlocker3D
    {
        public bool IsBlocking => true;

        public event Action HitBlocked;

        public void OnHitBlocked(HitData3D hitData)
        {
            HitBlocked?.Invoke();
        }
    }
}
