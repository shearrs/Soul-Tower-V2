using Shears;
using Shears.HitDetection;
using UnityEngine;

public class TemporaryAudio : MonoBehaviour
{
    private readonly Timer deleteTimer = new();

    [SerializeField] private AudioSource soundPlayer;

    private void Awake()
    {
        deleteTimer.Completed += () => Destroy(gameObject);
    }

    public void PlayAudio(AudioClip clip)
    {
        soundPlayer.clip = clip;
        soundPlayer.Play();
        deleteTimer.Start(clip.length);
    }
}
