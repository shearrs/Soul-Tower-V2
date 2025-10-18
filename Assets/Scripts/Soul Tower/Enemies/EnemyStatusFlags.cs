using UnityEngine;

namespace SoulTower.Enemies
{
    [System.Serializable]
    public class EnemyStatusFlags
    {
        [SerializeField] private bool isWet;

        public bool IsWet { get; internal set; }
    }
}
