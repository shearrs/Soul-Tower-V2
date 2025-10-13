using Shears;
using Shears.Logging;
using UnityEngine;

namespace SoulTower.Towers
{
    public abstract class SpecialTileGroup : SHMonoBehaviourLogger
    {
        [SerializeField, ReadOnly] private string groupID;

        public string GroupID { get => groupID; set => groupID = value; }
        public abstract int Count { get; }
    }
}
