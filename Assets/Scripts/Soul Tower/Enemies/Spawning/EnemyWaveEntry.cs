using UnityEngine;

namespace SoulTower.Enemies
{
    [System.Serializable]
    public struct EnemyWaveEntry
    {
        [SerializeField] private Enemy enemyPrefab;
        [SerializeField, Min(1)] private int count;

        public readonly Enemy EnemyPrefab => enemyPrefab;
        public int Count { readonly get => count; set => count = value; }

        public EnemyWaveEntry(Enemy enemyPrefab, int count)
        {
            this.enemyPrefab = enemyPrefab;
            this.count = count;
        }
    }
}
