using SoulTower.Towers;
using Shears;
using UnityEngine;
using System;

namespace SoulTower.Traps
{
    public class Boulder : MonoBehaviour
    {
        private const float MIN_LIFETIME = 0.25f;

        [SerializeField] private GameObject shardPrefab;
        [SerializeField] private Transform shardSpawn;

        private readonly Timer lifeTimer = new(MIN_LIFETIME);

        public event Action Collided;

        private void Start()
        {
            lifeTimer.Start();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(lifeTimer.IsDone)
            {
                GameObject shards = Instantiate(shardPrefab);
                shards.transform.position = shardSpawn.transform.position;

                //GameObject soundPlayer = Instantiate(tempAudio);
                //soundPlayer.GetComponent<TemporaryAudio>().PlayAudio(boulderAudio);

                Collided?.Invoke();
                Destroy(gameObject);
            }
        }
    }
}
