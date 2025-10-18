using Shears.HitDetection;
using SoulTower.HitDetection;
using System;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class DefenderShieldHitReceiver : HitReceiver3D, IHitBlocker3D
    {
        public event Action HitBlocked;

        // do something here
        //protected override void ReceiveHit(HitData3D hitData)
        //{
        //    foreach (var data in hitData.Data)
        //    {
        //        if (data is DamageData damageData)
        //        {

        //        }
        //    }
        //}
        public bool IsBlocking => true;

        public void OnHitBlocked(HitData3D hitData)
        {
            HitBlocked?.Invoke();
        }
    }
}
