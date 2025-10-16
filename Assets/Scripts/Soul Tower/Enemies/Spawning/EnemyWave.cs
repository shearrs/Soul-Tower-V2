using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Enemies
{
    [System.Serializable]
    public class EnemyWave
    {
        [SerializeField] private EnemyWaveEntry[] entries;

        public IReadOnlyCollection<EnemyWaveEntry> EnemyEntries => entries;
    }
}
