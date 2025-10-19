using UnityEngine;

namespace SoulTower.Enemies
{
    [System.Serializable]
    public class EnemyStatusFlags
    {
        [SerializeField] private bool isWet;
        [SerializeField] private bool isCold;

        public bool IsWet { get => isWet; internal set => isWet = value; }
        public bool IsCold { get => isCold; internal set => isCold = value; }
    }
}
