using Shears.Pathfinding;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Towers
{
    public abstract class TowerNodeData : PathNodeData
    {
        private readonly HashSet<IPathEntity> entities = new();

        public int EntityCount => entities.Count;

        public bool ContainsEntity(IPathEntity entity) => entities.Contains(entity);

        public void RegisterEntity(IPathEntity entity)
        {
            if (!entities.Contains(entity))
                entities.Add(entity);
        }

        public void DeregisterEntity(IPathEntity entity) => entities.Remove(entity);
    }
}
