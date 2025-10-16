using UnityEngine;

namespace SoulTower.Enemies
{
    public class EnemyModel : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float rotationSpeed = 1.0f;

        public Animator Animator => animator;
        public float RotationSpeed => rotationSpeed;
    }
}
