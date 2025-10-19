using Shears.Detection;
using UnityEngine;
using SoulTower.Enemies;

namespace SoulTower.Players
{
    public class SoulInteractor : MonoBehaviour
    {
        [SerializeField] private AreaDetector3D detector;

        private void Update()
        {   
            detector.Detect();
            for(int i = 0; i < detector.Hits; i++)
            {
                var hit = detector.GetDetection(i);
                if (hit.TryGetComponent(out DroppedSoul soul))
                {
                    soul.Collect();
                }
            }
        }
    }
}
