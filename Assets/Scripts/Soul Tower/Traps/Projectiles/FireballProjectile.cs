using UnityEngine;

namespace SoulTower.Traps
{
    public class FireballProjectile : MonoBehaviour
    {
        //[SerializeField] private Transform trail;

        public float speed;
        public float rotationRate;

        void Start()
        {
            
        }

        void Update()
        {
            gameObject.transform.Translate(Vector3.forward * speed * Time.deltaTime);
            if (transform.forward != Vector3.down)
            {
                gameObject.transform.Rotate(Vector3.right * rotationRate * Time.deltaTime);
            }
        }

        public void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.name != "Interaction")
            {
                Destroy(gameObject);
            }
        }

        public void OnDestroy()
        {
            
        }
    }
}
