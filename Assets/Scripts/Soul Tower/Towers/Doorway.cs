using UnityEngine;

namespace SoulTower.Towers
{
    public class Doorway : MonoBehaviour
    {
        [SerializeField] private Vector3 entrancePosition;
        [SerializeField] private Vector3 stairsPosition;
        [SerializeField] private Vector3 exitPosition;

        public Vector3 EntrancePosition => transform.TransformPoint(entrancePosition);
        public Vector3 StairsPosition => transform.TransformPoint(stairsPosition);
        public Vector3 ExitPosition => transform.TransformPoint(exitPosition);

        private void OnDrawGizmosSelected()
        {
            var color = Color.yellow;
            color.a = 0.85f;
            Gizmos.color = color;
            Gizmos.DrawSphere(EntrancePosition, 0.15f);

            color = Color.orangeRed;
            color.a = 0.85f;
            Gizmos.color = color;
            Gizmos.DrawSphere(StairsPosition, 0.15f);

            color = Color.red;
            color.a = 0.85f;
            Gizmos.color = color;
            Gizmos.DrawSphere(ExitPosition, 0.15f);

            Gizmos.color = Color.white;
            Gizmos.DrawLine(EntrancePosition, StairsPosition);
            Gizmos.DrawLine(StairsPosition, ExitPosition);
        }
    }
}
