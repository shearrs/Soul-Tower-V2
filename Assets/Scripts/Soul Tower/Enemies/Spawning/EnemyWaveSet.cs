using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Enemies
{
    [CreateAssetMenu(menuName = "Soul Tower/Enemy Wave Set")]
    public class EnemyWaveSet : ScriptableObject
    {
        [SerializeField] private EnemyWave[] waves;

        public IReadOnlyCollection<EnemyWave> Waves => waves;
    }
}
