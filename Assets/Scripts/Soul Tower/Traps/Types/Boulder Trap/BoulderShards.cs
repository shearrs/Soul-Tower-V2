using UnityEngine;

namespace SoulTower.Traps
{
    public class BoulderShards : MonoBehaviour
    {
        [SerializeField] private ParticleSystem effect1;
        [SerializeField] private ParticleSystem effect2;
        [SerializeField] private ParticleSystem effect3;

        private float lifetime;
        private void Start()
        {
            effect1.Play();
            effect2.Play();
            effect3.Play();
        }
        void Update()
        {
            lifetime += Time.deltaTime;
            if(lifetime > 6f)
            {
                Destroy(gameObject);
            }
        }
    }
}
