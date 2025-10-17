using Shears;
using Shears.Beziers;
using UnityEngine;
using System;
using UnityEngine.Serialization;

namespace SoulTower.Traps.UI
{
    public class ShockSurfaceLightning : MonoBehaviour
    {
        [Header("Line Renderer")]
        [SerializeField] private Bezier bezier;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField, Range(2, 32)] private int resolution;

        [Header("Bezier")]
        [SerializeField] private bool isBack;
        [SerializeField] private bool isRight;
        [SerializeField, Min(0.001f)] private float switchInterval;
        [SerializeField, FormerlySerializedAs("BezierX")] private Range<float> bezierX;
        [SerializeField, FormerlySerializedAs("BezierY")] private Range<float> bezierY;
        [SerializeField, FormerlySerializedAs("BezierZfront")] private Range<float> bezierZFront;
        [SerializeField, FormerlySerializedAs("BezierZback")] private Range<float> bezierZBack;

        private readonly Timer switchTimer = new();
        private bool isEnabled = false;

        private void OnValidate()
        {
            if (lineRenderer == null)
                return;
            else
                lineRenderer.positionCount = resolution;
        }

        public void Enable()
        {
            if (isEnabled)
                return;

            lineRenderer.enabled = true;

            switchTimer.Start(switchInterval);
            switchTimer.Completed += SwitchBolts;

            SwitchBolts();
            isEnabled = true;
        }

        public void Disable()
        {
            if (!isEnabled)
                return;

            lineRenderer.enabled = false;

            switchTimer.Stop();
            switchTimer.Completed -= SwitchBolts;

            isEnabled = false;
        }

        private void SwitchBolts()
        {
            float X;
            float Y;
            float Z;

            foreach (var point in bezier.Points)
            {
                X = bezierX.Random();
                Y = bezierY.Random();
                if (isBack)
                {
                    Z = bezierZBack.Random();
                }
                else
                {
                    Z = bezierZFront.Random();
                }
                if (isRight)
                {
                    Z *= -1;
                }

                point.LocalTangent1 = new Vector3(X, Y, Z);
            }

            for (int i = 0; i < resolution; i++)
            {
                float t = (float)i / (resolution - 1);
                Vector3 pos = bezier.Sample(t);

                lineRenderer.SetPosition(i, pos);
            }

            switchTimer.Start(switchInterval);
        }
    }
}
