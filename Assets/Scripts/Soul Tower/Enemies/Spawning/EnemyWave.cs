using Shears;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Enemies
{
    [System.Serializable]
    public class EnemyWave
    {
        [SerializeField] private Range<float> spawnRateRange = new(0.5f, 2.0f);
        [SerializeField] private EnemyWaveEntry[] entries;

        public Range<float> SpawnRateRange => spawnRateRange;
        public IReadOnlyCollection<EnemyWaveEntry> EnemyEntries => entries;
    }
}
