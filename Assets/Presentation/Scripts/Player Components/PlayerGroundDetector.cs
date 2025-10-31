using UnityEngine;

namespace Presentation
{
    public class PlayerGroundDetector : MonoBehaviour
    {
        public PlayerFlags flags;
        public Vector3 offset;
        public float radius;
        public LayerMask layers = 1;

        private void Update()
        {
            UpdateIsGrounded();
        }

        private void UpdateIsGrounded()
        {
            flags.isGrounded = Physics.CheckSphere(transform.position + offset, radius, layers);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + offset, radius);
        }
    }
}
