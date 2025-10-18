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
        [SerializeField] private Transform leftSpawnPoint;
        [SerializeField] private Transform rightSpawnPoint;
        [SerializeField] private Transform enemyContainer;

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
                var enemy = Instantiate(entry.EnemyPrefab, enemyContainer);

                if (entry.Count - 1 == 0)
                    entries.Remove(entry);
                else
                {
                    entry.Count--;
                    entries[enemyIndex] = entry;
                }

                Transform targetSpawn;

                if ((enemy.SpawnFlags & EnemySpawnFlags.Right) != 0)
                {
                    if ((enemy.SpawnFlags & EnemySpawnFlags.Left) == 0)
                        targetSpawn = rightSpawnPoint;
                    else
                    {
                        int sideRandom = Random.Range(0, 2);

                        if (sideRandom == 0)
                            targetSpawn = leftSpawnPoint;
                        else
                            targetSpawn = rightSpawnPoint;
                    }
                }
                else if ((enemy.SpawnFlags & EnemySpawnFlags.SideWithOpening) != 0)
                {
                    bool rightOpening = tower.HasRightOpening();
                    bool leftOpening = tower.HasLeftOpening();

                    if (rightOpening && leftOpening)
                    {
                        int sideRandom = Random.Range(0, 2);

                        if (sideRandom == 0)
                            targetSpawn = leftSpawnPoint;
                        else
                            targetSpawn = rightSpawnPoint;
                    }
                    else
                        targetSpawn = rightOpening ? rightSpawnPoint : leftSpawnPoint;
                }
                else
                    targetSpawn = leftSpawnPoint;

                int zOffset = targetSpawn == leftSpawnPoint ? Random.Range(0, 2) : 0;

                enemy.transform.SetPositionAndRotation(targetSpawn.position + Vector3.forward * zOffset, targetSpawn.rotation);
                enemy.Spawn(tower);

                yield return CoroutineUtil.WaitForSeconds(wave.SpawnRateRange.Random());
            }
        }
    }
}
