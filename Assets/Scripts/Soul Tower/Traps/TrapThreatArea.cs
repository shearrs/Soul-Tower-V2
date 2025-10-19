using Shears;
using Shears.HitDetection;
using Shears.Logging;
using UnityEngine;

namespace SoulTower.Traps
{
    public class TrapThreatArea : MonoBehaviour
    {
        [SerializeField] private bool drawGizmosAlways = false;
        [SerializeField] private Trap trap;
        [SerializeField] private HitBody3D hitBody;
        [SerializeField] private Collider[] colliders;
        [SerializeField, Min(0.0f)] private float extraDuration = 0.15f;

        private bool isActive = false;
        private readonly Timer durationTimer = new();

        public bool IsPrimed => !trap.IsOnCooldown;
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

        private void SetLayer()
        {
            foreach (var col in colliders)
                col.gameObject.layer = LayerMask.NameToLayer("Enemy Detections");
        }

        public void Enable()
        {
            isActive = true;
        }

        public void Disable()
        {
            isActive = false;
        }

        public Vector3 GetLeft()
        {
            if (colliders.Length == 0)
            {
                SHLogger.Log("Threat area has no colliders!", SHLogLevels.Error);
                return Vector3.zero;
            }

            Bounds mostLeftBounds = colliders[0].bounds;

            for (int i = 1; i < colliders.Length; i++)
            {
                var col = colliders[i];

                if (col.bounds.min.x < mostLeftBounds.min.x)
                    mostLeftBounds = col.bounds;
            }

            return mostLeftBounds.min;
        }

        public Vector3 GetRight()
        {
            if (colliders.Length == 0)
            {
                SHLogger.Log("Threat area has no colliders!", SHLogLevels.Error);
                return Vector3.zero;
            }

            Bounds mostRightBounds = colliders[0].bounds;

            for (int i = 1; i < colliders.Length; i++)
            {
                var col = colliders[i];

                if (col.bounds.max.x > mostRightBounds.max.x)
                    mostRightBounds = col.bounds;
            }

            return mostRightBounds.max;
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
