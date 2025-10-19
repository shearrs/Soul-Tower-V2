using Shears.HitDetection;
using UnityEngine;

namespace SoulTower.Traps
{
    public class DartProjectile : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float rotationRate;
        [SerializeField] private TrapProjectileHitDeliverer deliverer;

        private void OnEnable()
        {
            deliverer.HitDelivered += OnHitDelivered;
        }
        private void OnDisable()
        {
            deliverer.HitDelivered -= OnHitDelivered;
        }

        private void OnHitDelivered(HitData3D _)
        {
            Destroy(gameObject);
        }

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
            Destroy(gameObject);
        }
    }
}
