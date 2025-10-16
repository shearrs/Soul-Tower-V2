using UnityEngine;

namespace SoulTower.Enemies
{
    public class EnemyModel : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField, Min(0.0f)] private float walkPlaybackSpeed = 0.85f;
        [SerializeField, Min(0.0f)] private float rotationSpeed = 1.0f;

        public Animator Animator => animator;
        public float WalkPlaybackSpeed => walkPlaybackSpeed;
        public float RotationSpeed => rotationSpeed;
    }
}
