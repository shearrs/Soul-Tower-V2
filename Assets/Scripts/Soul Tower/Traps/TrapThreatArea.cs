using Shears;
using Shears.HitDetection;
using UnityEngine;

namespace SoulTower.Traps
{
    public class TrapThreatArea : MonoBehaviour
    {
        [SerializeField] private bool drawGizmosAlways = false;
        [SerializeField] private HitBody3D hitBody;
        [SerializeField, Min(0.0f)] private float extraDuration = 0.15f;
        [SerializeField] private Collider[] colliders;

        private bool isActive = false;
        private readonly Timer durationTimer = new();

        public bool IsActive => isActive;

        private void Awake()
        {
            durationTimer.Completed += Disable;
        }

        private void OnEnable()
        {
            hitBody.Enabled += Enable;
            hitBody.Disabled += OnHitBodyDisabled;
        }

        private void OnDisable()
        {
            durationTimer.Stop();

            hitBody.Enabled -= Enable;
            hitBody.Disabled -= OnHitBodyDisabled;
        }

        private void OnValidate()
        {
            foreach (var col in colliders)
                col.isTrigger = true;

            Invoke(nameof(SetLayer), 0f);
        }

        public void Enable()
        {
            foreach (var col in colliders)
                col.enabled = true;

            isActive = true;
        }

        public void Disable()
        {
            foreach (var col in colliders)
                col.enabled = false;

            isActive = false;
        }

        private void SetLayer()
        {
            foreach (var col in colliders)
                col.gameObject.layer = LayerMask.NameToLayer("Enemy Detections");
        }

        private void OnHitBodyDisabled()
        {
            if (extraDuration == 0.0f)
                Disable();
            else
                durationTimer.Restart(extraDuration);
        }

        private void OnDrawGizmos()
        {
            if (drawGizmosAlways)
                DrawGizmos();
        }

        private void OnDrawGizmosSelected()
        {
            if (!drawGizmosAlways)
                DrawGizmos();
        }

        private void DrawGizmos()
        {
            var color = Color.mediumVioletRed;
            color.a = 0.5f;
            Gizmos.color = color;

            foreach (var col in colliders)
            {
                if (col == null || !col.enabled)
                    continue;

                if (col is BoxCollider boxCol)
                    Gizmos.DrawCube(transform.TransformPoint(boxCol.center), transform.lossyScale.MultiplyComponents(boxCol.size));
            }
        }
    }
}
