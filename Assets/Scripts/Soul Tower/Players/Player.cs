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

        private void OnEnable()
        {
            tower.RoomsChanged += OnRoomsChanged;
        }

        private void OnDisable()
        {
            tower.RoomsChanged -= OnRoomsChanged;
        }

        private void OnRoomsChanged()
        {
            cam.UpdateMaxScrollHeight();
        }
    }
}
