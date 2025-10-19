using SoulTower.Towers;
using Shears;
using UnityEngine;

namespace SoulTower.Traps
{
    public class Boulder : MonoBehaviour
    {
        private const float MIN_LIFETIME = 1.0f;

        [SerializeField] private GameObject shardPrefab;
        [SerializeField] private Transform shardSpawn;

        private readonly Timer lifeTimer;

        private void Start()
        {
            lifeTimer.Start(MIN_LIFETIME);
        }

        private void OnTriggerEnter(Collider other)
        {
            if(lifeTimer.IsDone)
            {
                GameObject shards = Instantiate(shardPrefab);
                shards.transform.position = shardSpawn.transform.position;
                Destroy(gameObject);
            }
        }
    }
}
