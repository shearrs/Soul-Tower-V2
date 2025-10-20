using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Towers.UI
{
    public class CatalystParticles : MonoBehaviour
    {
        [SerializeField] private Catalyst catalyst;
        [SerializeField] private List<ParticleSystem> damageParticles;

        private int currentHealth;

        private void OnEnable()
        {
            currentHealth = catalyst.Health;
            catalyst.HealthChanged += PlayDamagedParticles;
        }

        private void OnDisable()
        {
            catalyst.HealthChanged -= PlayDamagedParticles;
        }

        private void PlayDamagedParticles(int newHealth)
        {
            if(currentHealth > newHealth)
            {
                foreach (var sys in damageParticles)
                    sys.Play();
            }

            currentHealth = newHealth;
        }
    }
}
