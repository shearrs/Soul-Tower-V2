using Shears;
using Shears.HitDetection;
using UnityEngine;

namespace SoulTower.Traps
{
    public class FireballProjectile : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private GameObject explosionPrefab;
        [SerializeField] private GameObject model;

        [Header("Hit Detection")]
        [SerializeField] private TrapHitDeliverer hitDeliverer;
        [SerializeField] private HitBody3D hitBody;
        [SerializeField] private HitBody3D explosionHitBody;

        [Header("Settings")]
        [SerializeField] private float speed;
        [SerializeField] private float rotationRate;
        [SerializeField] private float explosionLingerTime = 0.15f;

        private readonly Timer explosionTimer = new();

        private void Awake()
        {
            hitDeliverer.HitDelivered += _ => Explode();

            explosionTimer.Completed += OnExplosionTimerCompleted;
        }

        void Update()
        {
            transform.Translate(speed * Time.deltaTime * Vector3.forward);

            if (transform.forward != Vector3.down)
                transform.Rotate(rotationRate * Time.deltaTime * Vector3.right);
        }

        private void Explode()
        {
            GameObject explosion = Instantiate(explosionPrefab);
            explosion.transform.position = transform.position;
            model.SetActive(false);
            hitBody.enabled = false;
            explosionHitBody.enabled = true;

            explosionTimer.Start(explosionLingerTime);
        }

        private void OnExplosionTimerCompleted()
        {
            Destroy(gameObject);
        }

        public void OnTriggerEnter(Collider other)
        {
            if (!explosionTimer.IsDone)
                return;

            Explode();
        }
    }
}
