using Shears;
using Shears.Tweens;
using Shears.Beziers;
using System.Collections;
using UnityEngine;
using System;

namespace SoulTower.Traps.UI
{
    public class ShockSurfaceLightning : MonoBehaviour
    {
        [SerializeField] private Bezier bolt;
        [SerializeField] private LineRenderer boltLine;
        [SerializeField, Range(2, 32)] private int resolution;

        [SerializeField] private bool isBack;
        [SerializeField] private bool isRight;
        [SerializeField] private float switchInterval;
        [SerializeField] private Range<float> BezierX;
        [SerializeField] private Range<float> BezierY;
        [SerializeField] private Range<float> BezierZfront;
        [SerializeField] private Range<float> BezierZback;


        private float switchTimer;

        private void OnValidate()
        {
            if(boltLine == null)
            {
                return;
            } else
            {
                boltLine.positionCount = resolution;
            }
        }

        private void Update()
        {
            switchTimer += Time.deltaTime;
            
            for (int i = 0; i < resolution; i++)
            {
                float t = (float)i / (resolution - 1);
                Vector3 pos = bolt.Sample(t);

                boltLine.SetPosition(i, pos);
            }

            if(switchTimer > switchInterval)
            {
                float X;
                float Y;
                float Z;

                foreach (var point in bolt.Points)
                {
                    X = BezierX.Random();
                    Y = BezierY.Random();
                    if (isBack)
                    {
                        Z = BezierZback.Random();
                    } else
                    {
                        Z = BezierZfront.Random();
                    }
                    if (isRight)
                    {
                        Z *= -1;
                    }

                    point.LocalTangent1 = new Vector3(X, Y, Z);
                }

                switchTimer = 0;
            }
        }
    }
}
