using SoulTower.Towers;
using UnityEngine;

namespace SoulTower.Players
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Tower tower;
        [SerializeField] private PlayerCamera cam;

        private void Awake()
        {
            cam.SetTower(tower);
        }
    }
}
