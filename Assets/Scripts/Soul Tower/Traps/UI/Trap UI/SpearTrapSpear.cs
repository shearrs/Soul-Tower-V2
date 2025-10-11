using Shears;
using Shears.Tweens;
using UnityEngine;

namespace SoulTower.Traps.UI
{
    public class SpearTrapSpear : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Transform pole;
        [SerializeField] private Transform spear;

        [Header("Settings")]
        [SerializeField] private float spearOffset;
        [SerializeField] private float startScale;
        [SerializeField] private float endScale;
        [SerializeField] private float baseHeight;

        public void SetPosition(float t)
        {
            Vector3 scale = pole.transform.localScale;
            scale.y = Mathf.LerpUnclamped(startScale, endScale, t);

            Vector3 position = pole.transform.localPosition;
            position.y = baseHeight + scale.y;

            Vector3 spearPosition = spear.transform.localPosition;
            spearPosition.y = position.y + scale.y + spearOffset;

            pole.transform.localScale = scale;
            pole.transform.localPosition = position;
            spear.transform.localPosition = spearPosition;
        }
    }
}
