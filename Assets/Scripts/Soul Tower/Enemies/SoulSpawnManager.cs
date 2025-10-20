using Shears.Signals;
using SoulTower.Currency;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class SoulSpawnManager : MonoBehaviour
    {
        [SerializeField] private GameObject catalyst;
        [SerializeField] private DroppedSoul soulPrefab;

        void OnEnable()
        {
            SignalShuttle.Register<EnemyDiedSignal>(OnEnemyDied);
        }
        
        void OnDisable()
        {
            SignalShuttle.Deregister<EnemyDiedSignal>(OnEnemyDied);
        }

        //TODO: Get combo amount
        private void OnEnemyDied(EnemyDiedSignal signal)
        {
            Vector3 spawnPos = signal.Enemy.transform.position;
            DroppedSoul soul = Instantiate(soulPrefab);

            soul.transform.position = spawnPos + new Vector3(0f, 0.7f, 0f);
            soul.CatalystLocation = catalyst.transform.position + new Vector3(0f, 1.5f, 0f);
        }
    }
}
