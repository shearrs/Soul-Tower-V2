using Shears;
using Shears.Tweens;
using SoulTower.Towers;
using System.Collections;
using UnityEngine;

namespace SoulTower.Traps.UI
{
    public class FlamethrowerAnimation : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private FlamethrowerTrap flamethrower;
        [SerializeField] private Transform nozzle;
        [SerializeField] private ParticleSystem fire1;
        [SerializeField] private ParticleSystem fire2;
        [SerializeField] private ParticleSystem ember;

        [Header("Audio")]
        [SerializeField] private AudioSource trapAudio;
        [SerializeField] private AudioClip activeClip;

        [SerializeField] private TweenData shakeTweenData;

        private Tween tween;

        private void OnEnable()
        {
            flamethrower.Activated += OnFlamethrowerActivated;
        }

        private void OnDisable()
        {
            flamethrower.Activated -= OnFlamethrowerActivated;
        }

        private void OnFlamethrowerActivated(Trap _)
        {
            trapAudio.clip = activeClip;
            trapAudio.Play();
            
            fire1.Play();
            fire2.Play();
            ember.Play();

            tween = nozzle.DoShakeTween(.05f, 0.1f, shakeTweenData);
            tween.Completed += () => tween.Dispose();
        }
    }
}
