using System.Collections;
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

        private readonly Timer activeTimer = new(2.0f);
        private readonly Timer flipTimer = new();
        private Material offMat;
        private Material onMat;
        private Color emissiveColor;

        private void Start()
        {
            onMat = Instantiate(padMesh.materials[1]);
            offMat = Instantiate(padMesh.materials[2]);

            padMesh.materials[1] = onMat;
            padMesh.materials[2] = offMat;

            emissiveColor = padMesh.materials[1].GetColor(EMISSION_COLOR_ID);
        }

        private void OnEnable()
        {
            shockTrap.Activated += OnShockSurfaceActivated;
            activeTimer.Completed += OnActiveTimerComplete;
        }

        private void OnDisable()
        {
            shockTrap.Activated -= OnShockSurfaceActivated;
            activeTimer.Completed -= OnActiveTimerComplete;
        }

        private void OnShockSurfaceActivated(Trap _)
        {
            foreach (var bolt in lightningBolts)
                bolt.Enable();

            padMesh.materials[1].SetColor(EMISSION_COLOR_ID, emissiveColor * -10);
            padMesh.materials[2].SetColor(EMISSION_COLOR_ID, emissiveColor * 3.5f);

            activeTimer.Restart();

            StopAllCoroutines();
            StartCoroutine(IEAnimate());
        }

        private IEnumerator IEAnimate()
        {
            activeTimer.Restart();

            while (true)
            {
                flipTimer.Restart(flickerInterval.Random());

                while (!flipTimer.IsDone)
                    yield return null;

                padMesh.materials[1].SetColor(EMISSION_COLOR_ID, emissiveColor * 3.5f);
                padMesh.materials[2].SetColor(EMISSION_COLOR_ID, emissiveColor * -10);

                flipTimer.Restart(flickerInterval.Random());

                while (!flipTimer.IsDone)
                    yield return null;

                padMesh.materials[1].SetColor(EMISSION_COLOR_ID, emissiveColor * -10);
                padMesh.materials[2].SetColor(EMISSION_COLOR_ID, emissiveColor * 3.5f);

                yield return null;
            }
        }

        private void OnActiveTimerComplete()
        {
            StopAllCoroutines();
            DeactivateBolts();
        }

        private void DeactivateBolts()
        {
            foreach (var bolt in lightningBolts)
                bolt.Disable();

            padMesh.materials[1].SetColor("_EmissionColor", emissiveColor * 3.5f);
            padMesh.materials[2].SetColor("_EmissionColor", emissiveColor * -10);
        }
    }
}
