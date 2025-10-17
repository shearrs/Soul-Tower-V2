using System.Collections.Generic;
using Shears;
using UnityEngine;

namespace SoulTower.Traps.UI
{
    public class ShockSurfaceAnimation : MonoBehaviour
    {
        private static readonly int EMISSION_COLOR_ID = Shader.PropertyToID("_EmissionColor");

        [Header("Components")]
        [SerializeField] private ShockSurface shockTrap;
        [SerializeField] private List<ShockSurfaceLightning> lightningBolts;
        [SerializeField] private MeshRenderer padMesh;
        [SerializeField] private Range<float> flickerInterval;
        [SerializeField, Min(0.001f)] private float flickerTime;

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

            emissiveC = padMesh.materials[1].GetColor(EMISSION_COLOR_ID);

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
            foreach (var bolt in lightningBolts)
                bolt.Enable();

            padMesh.materials[1].SetColor(EMISSION_COLOR_ID, emissiveC * -10);
            padMesh.materials[2].SetColor(EMISSION_COLOR_ID, emissiveC * 3.5f);

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
                        padMesh.materials[1].SetColor(EMISSION_COLOR_ID, emissiveC * 3.5f);
                        padMesh.materials[2].SetColor(EMISSION_COLOR_ID, emissiveC * -10);
                        matFlippedTimer = 0;
                        matFlipped = true;
                        flipAtTime = flickerInterval.Random();
                    }
                } else
                {
                    matFlipBackTimer += Time.deltaTime;
                    if(matFlipBackTimer > flickerTime)
                    {
                        padMesh.materials[1].SetColor(EMISSION_COLOR_ID, emissiveC * -10);
                        padMesh.materials[2].SetColor(EMISSION_COLOR_ID, emissiveC * 3.5f);
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
            foreach (var bolt in lightningBolts)
                bolt.Disable();

            padMesh.materials[1].SetColor("_EmissionColor", emissiveC * 3.5f);
            padMesh.materials[2].SetColor("_EmissionColor", emissiveC * -10);

            trapActiveTime = 0f;
            trapActive = false;
        }
    }
}
