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
        [SerializeField] private GameObject catalyst;
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
            soul.transform.position = spawnPos + new Vector3(0f, 0.7f, 0f);
            soul.GetComponent<DroppedSoul>().CatalystLocation = catalyst.transform.position + new Vector3(0f, 1.5f, 0f);
        }
    }
}
