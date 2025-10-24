using Shears;
using SoulTower.Traps;
using UnityEngine;

namespace SoulTower.Audio
{
    public class DartAudio : MonoBehaviour
    {
        [SerializeField] private DartProjectile dart;
        [SerializeField] private AudioSource audioSource;

        private void OnEnable()
        {
            dart.Destroyed += OnCollided;
        }

        private void OnDisable()
        {
            dart.Destroyed -= OnCollided;
        }

        private void OnCollided()
        {
            transform.SetParent(null);

            audioSource.pitch = Random.Range(0.85f, 1.15f);
            audioSource.Play();

            CoroutineUtil.DoAfter(() => Destroy(gameObject), audioSource.clip.length);
        }
    }
}
