using Shears;
using Shears.Tweens;
using SoulTower.Towers;
using System.Collections;
using Shears.Signals;
using UnityEngine;

namespace SoulTower.Enemies.UI
{
    public class SoulSpawnManager : MonoBehaviour
    {
        [SerializeField] private Catalyst catalyst;
        [SerializeField] private GameObject soulPrefab;

        void OnEnable()
        {
            SignalShuttle.Register<EnemyDiedSignal>(OnEnemyDied);
        }
        
        void OnDisable()
        {
            SignalShuttle.Deregister<EnemyDiedSignal>(OnEnemyDied);
        }

        private void OnEnemyDied(EnemyDiedSignal signal)
        {
            Vector3 spawnPos = signal.Enemy.transform.position;
            //TODO: Get combo amount
            GameObject soul = Instantiate(soulPrefab);
            soul.transform.position = spawnPos;
        }
    }
}
