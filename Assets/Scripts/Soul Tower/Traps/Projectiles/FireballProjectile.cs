using UnityEngine;

namespace SoulTower.Traps
{
    public class FireballProjectile : MonoBehaviour
    {
        //[SerializeField] private Transform trail;

        public float speed;
        public float rotationRate;

        public GameObject explosionPrefab;

        void Start()
        {
            
        }

        void Update()
        {
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            if (transform.forward != Vector3.down)
            {
                transform.Rotate(Vector3.right * rotationRate * Time.deltaTime);
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.gameObject.name);
            GameObject explosion = Instantiate(explosionPrefab);
            explosion.transform.position = transform.position;
            Destroy(gameObject);
        }
    }
}
