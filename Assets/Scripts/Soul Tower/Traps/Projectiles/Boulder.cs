using SoulTower.Towers;
using UnityEngine;

namespace SoulTower.Traps
{
    public class Boulder : MonoBehaviour
    {
        [SerializeField] private GameObject shardPrefab;
        [SerializeField] private Transform shardSpawn;
        private float lifetime;

        private void Update()
        {
            lifetime += Time.deltaTime;
        }
        private void OnTriggerEnter(Collider other)
        {
            if(lifetime >= 1f)
            {
                if(other.gameObject.GetComponentInParent<Tile>() != null)
                {
                    GameObject shards = Instantiate(shardPrefab);
                    shards.transform.position = shardSpawn.transform.position;
                    Destroy(gameObject);
                }
            }
        }
    }
}
