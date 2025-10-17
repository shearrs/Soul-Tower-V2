using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;


namespace SoulTower.Traps.UI
{
    public class FreezeBreezeParticleAnimation : MonoBehaviour
    {
        [SerializeField] private ParticleSystem partSys;
        private ParticleSystem.Particle[] particles;

        //public float size = 1f;
        //public Color invisibleColor;

        private void Start()
        {
            particles = new ParticleSystem.Particle[partSys.main.maxParticles];
        }
        // Update is called once per frame
        void Update()
        {
            int numParticlesAlive = partSys.GetParticles(particles);

            for (int i = 0; i < numParticlesAlive; i++)
            {
                if (particles[i].position.z > 12f)
                {
                    particles[i].remainingLifetime = 0f;
                    partSys.SetParticles(particles, numParticlesAlive);
                }
            }
        }
    }
}
