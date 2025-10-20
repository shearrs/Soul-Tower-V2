using System.Collections;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip musicIntro;
    [SerializeField] private AudioClip musicLoop;

    void Start()
    {
        StartCoroutine(PlayAudioSequence());
    }

    private IEnumerator PlayAudioSequence()
    {
        audioSource.clip = musicIntro;
        audioSource.Play();

        yield return new WaitWhile(() => audioSource.isPlaying);
        audioSource.clip = musicLoop;
        audioSource.loop = true;
        audioSource.Play();
    }
}
