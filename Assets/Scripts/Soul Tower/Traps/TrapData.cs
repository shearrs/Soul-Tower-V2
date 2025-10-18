using Shears;
using SoulTower.HitDetection;
using SoulTower.Towers;
using UnityEngine;

namespace SoulTower.Traps
{
    [CreateAssetMenu(fileName = "New Trap Data", menuName = "Soul Tower/Trap Data")]
    public class TrapData : ScriptableObject
    {
        [Header("Activation")]
        [SerializeField] private bool isPassive = false;
        [SerializeField, ShowIf("!isPassive")] private float cooldown = 5f;

        [Header("Placement")]
        [SerializeField, Range(1, 4)] private int size = 1;
        [SerializeField] private TileType placementType = TileType.Floor;

        [Header("Damage")]
        [SerializeField] private DamageData[] damageData;

        public bool IsPassive => isPassive;
        public float Cooldown => cooldown;
        public int Size => size;
        public TileType PlacementType => placementType;
        public DamageData[] DamageData => damageData;
    }
}
