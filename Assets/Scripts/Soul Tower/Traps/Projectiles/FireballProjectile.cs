using UnityEngine;

namespace SoulTower.Traps
{
    public class FireballProjectile : MonoBehaviour
    {
        //[SerializeField] private Transform trail;

        [SerializeField] private GameObject explosionPrefab;
        [SerializeField] private float speed;
        [SerializeField] private float rotationRate;

        void Update()
        {
            transform.Translate(speed * Time.deltaTime * Vector3.forward);

            if (transform.forward != Vector3.down)
            {
                transform.Rotate(rotationRate * Time.deltaTime * Vector3.right);
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.gameObject.name, other.gameObject);

            GameObject explosion = Instantiate(explosionPrefab);
            explosion.transform.position = transform.position;

            Destroy(gameObject);
        }
    }
}
