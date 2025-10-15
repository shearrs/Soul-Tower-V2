using Shears.HitDetection;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class EnemyHitReceiver : MonoBehaviour, IHitReceiver3D
    {
        Transform IHitReceiver.Transform => transform;

        void IHitReceiver<HitData3D>.OnHitReceived(HitData3D hitData)
        {
            Destroy(gameObject);
        }
    }
}
