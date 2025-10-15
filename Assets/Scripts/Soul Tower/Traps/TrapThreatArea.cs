using Shears;
using Shears.HitDetection;
using UnityEngine;

namespace SoulTower.Traps
{
    public class TrapThreatArea : MonoBehaviour
    {
        [SerializeField] private Collider[] colliders;
        [SerializeField] private HitBody3D hitBody;

        private bool isActive = false;

        public bool IsActive => isActive;

        private void OnEnable()
        {
            hitBody.Enabled += Enable;
            hitBody.Disabled += Disable;
        }

        private void OnDisable()
        {
            hitBody.Enabled -= Enable;
            hitBody.Disabled -= Disable;
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

        private void OnDrawGizmosSelected()
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
