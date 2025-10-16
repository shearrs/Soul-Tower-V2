using Shears;
using Shears.Logging;
using SoulTower.Towers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class EnemySpawner : SHMonoBehaviourLogger
    {
        [Header("Spawner")]
        [SerializeField] private EnemyWaveSet waveSet;
        [SerializeField] private Tower tower;
        [SerializeField] private Transform spawnPoint;

        private readonly List<EnemyWaveEntry> entries = new();

        private void Start()
        {
            StartCoroutine(IESpawnWaves());
        }

        private IEnumerator IESpawnWaves()
        {
            if (waveSet == null)
            {
                Log("EnemySpawner requires a wave set to spawn!", SHLogLevels.Error);
                yield break;
            }

            foreach (var wave in waveSet.Waves)
                yield return IEWave(wave);
        }

        private IEnumerator IEWave(EnemyWave wave)
        {
            entries.Clear();
            entries.AddRange(wave.EnemyEntries);

            while (entries.Count > 0)
            {
                int enemyIndex = Random.Range(0, entries.Count);
                var entry = entries[enemyIndex];
                var enemy = Instantiate(entry.EnemyPrefab);

                if (entry.Count - 1 == 0)
                    entries.Remove(entry);
                else
                {
                    entry.Count--;
                    entries[enemyIndex] = entry;
                }

                int zOffset = Random.Range(0, 2);
                Vector3 spawnPosition = spawnPoint.position + Vector3.forward * zOffset;
                
                enemy.transform.position = spawnPosition;
                enemy.Spawn(tower);

                yield return CoroutineUtil.WaitForSeconds(wave.SpawnRateRange.Random());
            }
        }
    }
}
