using UnityEngine;

namespace SoulTower.Enemies
{
    [System.Serializable]
    public class EnemyStatusFlags
    {
        [SerializeField] private bool isWet;
        [SerializeField] private bool isCold;
        [SerializeField] private bool isSlimed;
        [SerializeField] private bool isShocked;

        public bool IsWet { get => isWet; internal set => isWet = value; }
        public bool IsCold { get => isCold; internal set => isCold = value; }
        public bool IsSlimed { get => isSlimed; internal set => isSlimed = value; }
        public bool IsShocked { get => isShocked; internal set => isShocked = value; }
    }
}
