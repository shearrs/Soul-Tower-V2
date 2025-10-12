using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Traps.UI
{
    public class TrapModel : MonoBehaviour
    {
        [SerializeField] private List<MeshRenderer> meshRenderers;

        private readonly Dictionary<MeshRenderer, Material> originalMaterials = new();

        private void Awake()
        {
            foreach (var renderer in meshRenderers)
                originalMaterials[renderer] = renderer.material;
        }

        public void SetMaterial(Material material)
        {
            foreach (var renderer in meshRenderers)
                renderer.material = material;
        }

        public void ResetMaterial()
        {
            foreach (var renderer in meshRenderers)
                renderer.material = originalMaterials[renderer];
        }
    }
}
