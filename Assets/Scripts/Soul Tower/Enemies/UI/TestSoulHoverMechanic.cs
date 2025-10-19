using UnityEngine;

namespace SoulTower.Enemies.UI
{
    public class TestSoulHoverMechanic : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private DroppedSoul soul;

        private bool collected = false;

        private void OnMouseEnter()
        {
            if (!collected)
            {
                soul.Collect();
                collected = true;
            }
        }
    }
}
