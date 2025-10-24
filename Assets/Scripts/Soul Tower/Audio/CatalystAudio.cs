using SoulTower.Towers;
using UnityEngine;

namespace SoulTower.Audio
{
    public class CatalystAudio : MonoBehaviour
    {
        [SerializeField] private Catalyst catalyst;
        [SerializeField] private AudioSource audioSource;

        private int currentHealth;

        private void OnEnable()
        {
            currentHealth = catalyst.Health;
            catalyst.HealthChanged += OnHealthChanged;
        }

        private void OnDisable()
        {
            catalyst.HealthChanged -= OnHealthChanged;
        }

        private void OnHealthChanged(int newHealth)
        {
            if (newHealth < currentHealth)
            {
                audioSource.pitch = Random.Range(0.85f, 1.15f);
                audioSource.Play();
            }

            currentHealth = newHealth;
        }
    }
}
