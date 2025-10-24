using Shears;
using SoulTower.Enemies;
using SoulTower.HitDetection;
using UnityEngine;

namespace SoulTower.Audio
{
    public class EnemyAudio : MonoBehaviour
    {
        [SerializeField] private EnemyHitReceiver hitReceiver;
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip pierceClip;
        [SerializeField] private AudioClip splatClip;

        private readonly Timer destroyTimer = new();

        private void OnEnable()
        {
            hitReceiver.DamageDataReceived += OnHitReceived;
        }

        private void OnDisable()
        {
            hitReceiver.DamageDataReceived -= OnHitReceived;
        }

        private void OnHitReceived(DamageData data)
        {
            if (data.Damage == 0 || !destroyTimer.IsDone)
                return;

            AudioClip clip;

            if (data.Type == DamageType.Crushing)
                clip = splatClip;
            else
                clip = pierceClip;

            transform.SetParent(null);
            audioSource.pitch = Random.Range(0.85f, 1.15f);
            audioSource.PlayOneShot(clip);

            destroyTimer.Start(clip.length);
            destroyTimer.Completed += () => Destroy(gameObject);
        }
    }
}
