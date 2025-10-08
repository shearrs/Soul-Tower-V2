using UnityEngine;

namespace SoulTower.Traps.UI
{
    public class TrapModel : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        private Material originalMaterial;

        private void Awake()
        {
            originalMaterial = meshRenderer.sharedMaterial;
        }

        public void SetMaterial(Material material)
        {
            meshRenderer.material = material;
        }

        public void ResetMaterial()
        {
            meshRenderer.material = originalMaterial;
        }
    }
}
