using Shears;
using Shears.HitDetection;
using Shears.Logging;
using UnityEngine;

namespace SoulTower.Traps
{
    public class TrapThreatArea : SHMonoBehaviourLogger
    {
        [Header("Threat Area")]
        [SerializeField] private bool drawGizmosAlways = false;
        [SerializeField] private Trap trap;
        [SerializeField] private HitBody3D hitBody;
        [SerializeField, Min(0.0f)] private float extraDuration = 0.15f;
        [SerializeField] private bool isBlocking = false;
        [SerializeField] private Collider[] colliders;

        private bool isActive = false;
        private readonly Timer durationTimer = new();

        public bool IsPrimed => !trap.IsOnCooldown;
        public bool IsActive => isActive;
        public bool IsBlocking => isBlocking;

        #region Initialization
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
            if (colliders == null)
                return;
            
            foreach (var col in colliders)
            {
                if (col != null)
                    col.isTrigger = true;
            }

            Invoke(nameof(SetLayer), 0f);
        }

        private void SetLayer()
        {
            foreach (var col in colliders)
            {
                if (col != null)
                    col.gameObject.layer = LayerMask.NameToLayer("Enemy Detections");
            }
        }

        public void Enable()
        {
            isActive = true;
        }

        public void Disable()
        {
            isActive = false;
        }

        private void OnHitBodyDisabled()
        {
            if (extraDuration == 0.0f)
                Disable();
            else
                durationTimer.Restart(extraDuration);
        }
        #endregion

        #region Positioning
        public void SetCenter(Vector3 center)
        {
            if (colliders == null || colliders.Length == 0)
            {
                Log("Threat area has no collider!", SHLogLevels.Error);
                return;
            }

            colliders[0].transform.position = center;
        }

        public Vector3 GetSize()
        {
            if (colliders == null || colliders.Length == 0)
            {
                Log("Threat area has no collider!", SHLogLevels.Error);
                return Vector3.zero;
            }

            if (colliders[0] is not BoxCollider box)
            {
                Log("Collider type is not implemented!", SHLogLevels.Error);
                return Vector3.zero;
            }

            return box.size;
        }

        public void SetSize(Vector3 size)
        {
            if (colliders == null || colliders.Length == 0)
            {
                Log("Threat area has no collider!", SHLogLevels.Error);
                return;
            }

            if (colliders[0] is not BoxCollider box)
            {
                Log("Collider type is not implemented!", SHLogLevels.Error);
                return;
            }

            box.size = size;
        }

        public Vector3 GetLeft()
        {
            if (colliders.Length == 0)
            {
                Log("Threat area has no colliders!", SHLogLevels.Error);
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
                Log("Threat area has no colliders!", SHLogLevels.Error);
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
        #endregion

        #region Gizmos
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
            if (colliders == null)
                return;

            var matrix = Gizmos.matrix;
            var color = Color.mediumVioletRed;
            color.a = isActive ? 0.85f : 0.25f;
            Gizmos.color = color;

            foreach (var col in colliders)
            {
                if (col == null || !col.enabled)
                    continue;

                Gizmos.matrix = Matrix4x4.TRS(col.transform.position, col.transform.rotation, col.transform.lossyScale);

                if (col is BoxCollider boxCol)
                    Gizmos.DrawCube(boxCol.center, boxCol.size);
            }

            Gizmos.matrix = matrix;
        }
        #endregion
    }
}
