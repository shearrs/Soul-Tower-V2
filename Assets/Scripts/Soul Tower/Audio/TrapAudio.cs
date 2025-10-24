using Shears;
using SoulTower.Traps;
using UnityEngine;

namespace SoulTower.Audio
{
    public class TrapAudio : MonoBehaviour
    {
        [SerializeField] private Trap trap;
        [SerializeField] private AudioSource audioSource;

        private void OnEnable()
        {
            trap.Activated += OnActivated;
        }

        private void OnDisable()
        {
            trap.Activated -= OnActivated;
        }

        private void OnActivated(Trap _)
        {
            audioSource.pitch = Random.Range(0.85f, 1.15f);
            audioSource.Play();
        }
    }
}
