using Shears.HitDetection;
using Shears.Logging;
using SoulTower.HitDetection;
using System;
using UnityEngine;

namespace SoulTower.Enemies
{
    [RequireComponent(typeof(EnemyStatusReceiver))]
    public class EnemyHitReceiver : HitReceiver3D
    {
        private Enemy enemy;
        private EnemyStatusReceiver statusReceiver;

        public event Action<DamageData> DamageDataReceived;

        private void Awake()
        {
            if (!TryGetComponent(out enemy))
                enemy = GetComponentInParent<Enemy>();

            if (enemy == null)
            {
                SHLogger.Log($"{nameof(EnemyHitReceiver)} needs to be attached or a child of an {nameof(Enemy)}!", SHLogLevels.Error);
                return;
            }

            if (!TryGetComponent(out statusReceiver))
                statusReceiver = GetComponentInParent<EnemyStatusReceiver>();

            if (statusReceiver == null)
            {
                SHLogger.Log($"{nameof(EnemyHitReceiver)} needs to be attached or a child of an {nameof(EnemyStatusReceiver)}!", SHLogLevels.Error);
                return;
            }
        }
         
        protected override void ReceiveHit(HitData3D hitData)
        {
            if (hitData.Data == null)
                return;

            foreach (var data in hitData.Data)
            {
                if (data is DamageData damageData)
                {
                    DamageDataReceived?.Invoke(damageData);

                    if (enemy.CanTakeDamageFrom(damageData))
                    {
                        if (damageData.Type == DamageType.Electric && enemy.StatusFlags.IsWet)
                            enemy.Die();
                        else
                            enemy.Damage(damageData.Damage);
                    }

                    foreach (var status in damageData.Statuses)
                        statusReceiver.Apply(status);
                }
            }
        }
    }
}
