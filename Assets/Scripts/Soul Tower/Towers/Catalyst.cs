using Shears;
using Shears.Logging;
using System.Collections.Generic;
using UnityEngine;

namespace SoulTower.Towers
{
    public class Catalyst : SHMonoBehaviourLogger
    {
        [Header("Catalyst")]
        [SerializeField] private Transform[] attackTransforms;

        private readonly List<AttackPoint> attackPoints = new();
        
        public class AttackPoint
        {
            private readonly Transform transform;
            private readonly HashSet<IPathEntity> entities = new();

            public Vector3 Position => transform.position;
            public Quaternion Rotation => transform.rotation;
            public int EntityCount => entities.Count;

            public AttackPoint(Transform transform)
            {
                this.transform = transform;
            }

            public void RegisterEntity(IPathEntity entity)
            {
                if (!entities.Contains(entity))
                    entities.Add(entity);
            }

            public void DeregisterEntity(IPathEntity entity)
            {
                entities.Remove(entity);
            }
        }

        private void Awake()
        {
            foreach (var transform in attackTransforms)
                attackPoints.Add(new(transform));
        }

        public Vector3 GetEntrancePosition()
        {
            if (attackPoints.Count == 0)
            {
                Log("Catalyst has no attack points!", SHLogLevels.Error);
                return transform.position;
            }

            int random = Random.Range(0, 2);

            return attackPoints[random].Position;
        }

        public AttackPoint GetEmptiestAttackPoint()
        {
            var lowestPoint = attackPoints[0];
            int lowestEntityCount = lowestPoint.EntityCount;

            for (int i = 1; i < attackPoints.Count; i++)
            {
                if (attackPoints[i].EntityCount < lowestEntityCount)
                {
                    lowestPoint = attackPoints[i];
                    lowestEntityCount = lowestPoint.EntityCount;
                }
            }

            return lowestPoint;
        }

        private void OnDrawGizmosSelected()
        {
            foreach (var point in attackPoints)
            {
                GizmosUtil.DrawText(point.Position, point.EntityCount.ToString());
            }
        }
    }
}
