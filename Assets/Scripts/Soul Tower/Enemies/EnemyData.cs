using Shears;
using UnityEngine;

namespace SoulTower.Enemies
{
    [CreateAssetMenu(fileName = "New Enemy Data", menuName = "Soul Tower/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        [SerializeField] private Range<float> moveSpeedRange = new(1, 2);

        public Range<float> MoveSpeedRange => moveSpeedRange;
    }
}
