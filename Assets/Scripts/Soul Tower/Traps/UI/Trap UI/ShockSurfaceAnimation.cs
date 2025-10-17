using System.Collections.Generic;
using Shears;
using Shears.Tweens;
using System.Collections;
using UnityEngine;
using System.Linq.Expressions;

namespace SoulTower.Traps.UI
{
    public class ShockSurfaceAnimation : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private ShockSurface shockTrap;
        [SerializeField] private List<LineRenderer> lightningBolts;
        [SerializeField] private MeshRenderer padMesh;
        [SerializeField] private Range<float> flickerInterval;
        [SerializeField] private float flickerTime;

        private Material offMat;
        private Material onMat;
        private Color emissiveC;

        private bool trapActive;
        private float trapActiveTime;
        private bool matFlipped = false;
        private float matFlippedTimer;
        private float flipAtTime;
        private float matFlipBackTimer;

        private void Start()
        {
            onMat = Instantiate(padMesh.materials[1]);
            offMat = Instantiate(padMesh.materials[2]);

            padMesh.materials[1] = onMat;
            padMesh.materials[2] = offMat;

            emissiveC = padMesh.materials[1].GetColor("_EmissionColor");

            flipAtTime = flickerInterval.Random();
        }

        private void OnEnable()
        {
            shockTrap.Activated += OnShockSurfaceActivated;
        }

        private void OnDisable()
        {
            shockTrap.Activated -= OnShockSurfaceActivated;
        }

        private void OnShockSurfaceActivated(Trap _)
        {
            foreach (var line in lightningBolts)
            {
                line.enabled = true;
            }

            padMesh.materials[1].SetColor("_EmissionColor", emissiveC * -10);
            padMesh.materials[2].SetColor("_EmissionColor", emissiveC * 3.5f);

            trapActive = true;
        }

        private void Update()
        {
            if (trapActive)
            {
                trapActiveTime += Time.deltaTime;

                if (!matFlipped)
                {
                    matFlippedTimer += Time.deltaTime;
                    if(matFlippedTimer > flipAtTime)
                    {
                        padMesh.materials[1].SetColor("_EmissionColor", emissiveC * 3.5f);
                        padMesh.materials[2].SetColor("_EmissionColor", emissiveC * -10);
                        matFlippedTimer = 0;
                        matFlipped = true;
                        flipAtTime = flickerInterval.Random();
                    }
                } else
                {
                    matFlipBackTimer += Time.deltaTime;
                    if(matFlipBackTimer > flickerTime)
                    {
                        padMesh.materials[1].SetColor("_EmissionColor", emissiveC * -10);
                        padMesh.materials[2].SetColor("_EmissionColor", emissiveC * 3.5f);
                        matFlipBackTimer = 0;
                        matFlipped = false;
                    }
                }

                if (trapActiveTime >= 2f)
                {
                    DeactivateTrap();
                }
            }
        }

        private void DeactivateTrap()
        {
            foreach (var line in lightningBolts)
            {
                line.enabled = false;
            }

            padMesh.materials[1].SetColor("_EmissionColor", emissiveC * 3.5f);
            padMesh.materials[2].SetColor("_EmissionColor", emissiveC * -10);

            trapActiveTime = 0f;
            trapActive = false;
        }
    }
}
