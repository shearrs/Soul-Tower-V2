using Shears.HitDetection;
using System;
using UnityEngine;

namespace SoulTower.Traps
{
    public class DartProjectile : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float rotationRate;
        [SerializeField] private TrapProjectileHitDeliverer deliverer;
        [SerializeField] private HitBody3D hitBody;

        public bool OnFire { get; private set; }

        public event Action LitOnFire;
        public event Action Destroyed;

        private void OnEnable()
        {
            deliverer.HitDelivered += OnHitDelivered;
        }

        private void OnDisable()
        {
            deliverer.HitDelivered -= OnHitDelivered;
        }

        private void Update()
        {
            transform.Translate(speed * Time.deltaTime * Vector3.forward);

            if (transform.forward != Vector3.down)
            {
                transform.Rotate(rotationRate * Time.deltaTime * Vector3.right);
            }
        }

        public void LightOnFire()
        {
            deliverer.AddData(new(1, HitDetection.DamageType.Fire));
            hitBody.CollisionMask = LayerMask.GetMask("Enemy", "Trap Receiver");
            OnFire = true;
            LitOnFire?.Invoke();
        }

        private void OnHitDelivered(HitData3D _)
        {
            Destroyed?.Invoke();
            Destroy(gameObject);
        }

        public void OnTriggerEnter(Collider other)
        {
            Destroyed?.Invoke();
            Destroy(gameObject);
        }
    }
}
