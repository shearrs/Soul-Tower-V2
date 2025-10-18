using UnityEngine;

namespace SoulTower.Enemies
{
    [RequireComponent(typeof(EnemyAnimator))]
    public class EnemyModel : MonoBehaviour
    {
        [SerializeField, Min(0.0f)] private float walkPlaybackSpeed = 0.75f;
        [SerializeField, Min(0.0f)] private float rotationSpeed = 150.0f;

        private EnemyAnimator animator;

        public EnemyAnimator Animator => animator;
        public float WalkPlaybackSpeed => walkPlaybackSpeed;
        public float RotationSpeed => rotationSpeed;

        private void Awake()
        {
            animator = GetComponent<EnemyAnimator>();
        }
    }
}
