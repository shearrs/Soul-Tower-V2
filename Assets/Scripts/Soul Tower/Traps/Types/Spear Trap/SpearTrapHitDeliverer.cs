using Shears.HitDetection;
using SoulTower.HitDetection;
using UnityEngine;

namespace SoulTower.Traps
{
    public class SpearTrapHitDeliverer : MonoBehaviour, IHitDeliverer3D
    {
        [SerializeField] private SpearTrap spearTrap;

        Transform IHitDeliverer.Transform => transform;

        IHitSubdata[] IHitDeliverer.GetCustomData()
        {
            return new IHitSubdata[] { new DamageData(1, DamageType.Piercing) };
        }

        void IHitDeliverer<HitData3D>.OnHitDelivered(HitData3D hitData)
        {
        }
    }
}
