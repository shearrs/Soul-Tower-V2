using Shears.Detection;
using UnityEngine;
using SoulTower.Currency;

namespace SoulTower.Players
{
    public class SoulInteractor : MonoBehaviour
    {
        [SerializeField] private AreaDetector3D detector;

        private void Update()
        {
            if (!detector.Detect())
                return;

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
