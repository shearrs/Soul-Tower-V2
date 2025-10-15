using Shears.Detection;
using SoulTower.Traps;
using UnityEngine;

namespace SoulTower.Enemies
{
    public static class EnemyNavigationUtil
    {
        public static bool DetectThreats(AreaDetector3D frontDetector, AreaDetector3D bodyDetector)
        {
            bool threat = false;

            if (frontDetector.Detect())
            {
                bodyDetector.Detect();

                if (frontDetector.TryGetDetection(out TrapThreatArea threatArea) && !bodyDetector.TryGetDetection(out TrapThreatArea _))
                    threat = threatArea.IsActive;
            }

            return threat;
        }
    }
}
