using UnityEngine;

namespace SoulTower.Towers
{
    public class WallOpening : MonoBehaviour
    {
        [SerializeField] private Vector3 entrancePosition;
        [SerializeField] private Vector3 fallStartPosition;
        [SerializeField] private Vector3 fallEndPosition;

        private Room room;

        public Room Room { get => room; internal set => room = value; }
        public Vector3 EntrancePosition => transform.TransformPoint(entrancePosition);
        public Vector3 FallStartPosition => transform.TransformPoint(fallStartPosition);
        public Vector3 FallEndPosition => transform.TransformPoint(fallEndPosition);

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(EntrancePosition, 0.15f);

            Gizmos.color = Color.orangeRed;
            Gizmos.DrawSphere(FallStartPosition, 0.15f);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(FallEndPosition, 0.15f);
        }
    }
}
