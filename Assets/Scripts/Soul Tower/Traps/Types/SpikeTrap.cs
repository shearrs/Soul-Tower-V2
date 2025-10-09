using Shears.HitDetection;
using UnityEngine;

namespace SoulTower.Traps
{
    [RequireComponent(typeof(Trap))]
    public class SpikeTrap : MonoBehaviour
    {
        [SerializeField] private HitBody3D hitBody;
        private Trap trap;

        private void Awake()
        {
            trap = GetComponent<Trap>();
        }

        private void OnEnable()
        {
            trap.Activated += OnActivated;
        }

        private void OnDisable()
        {
            trap.Activated -= OnActivated;
        }

        private void OnActivated()
        {
            hitBody.enabled = true;
        }
    }
}
