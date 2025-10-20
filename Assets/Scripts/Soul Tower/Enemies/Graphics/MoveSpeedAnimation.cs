using Shears.Logging;
using UnityEngine;

namespace SoulTower.Enemies
{
    public readonly struct MoveSpeedAnimation : IEnemyAnimation
    {
        private readonly Enemy enemy;
        private readonly int animationID;
        private readonly float speed;
        private readonly int layer;

        public readonly int ID => animationID;

        public readonly float Speed
        {
            get
            {
                if (enemy == null)
                {
                    SHLogger.Log("Enemy reference not set for speed animation!", SHLogLevels.Error);
                    return speed;
                }
                else if (enemy.Model == null)
                {
                    SHLogger.Log("Enemy has no model set!", SHLogLevels.Error);
                    return speed;
                }

                return speed * enemy.ResolvedMoveSpeed * enemy.Model.WalkPlaybackSpeed;
            }
        }

        public readonly int Layer => layer;

        public MoveSpeedAnimation(Enemy enemy, AnimationClip clip, float speed = 1.0f, int layer = 0)
        {
            animationID = Animator.StringToHash(clip.name);
            this.enemy = enemy;
            this.speed = speed;
            this.layer = layer;
        }

        public MoveSpeedAnimation(Enemy enemy, int id, float speed = 1.0f, int layer = 0)
        {
            this.enemy = enemy;
            animationID = id;
            this.speed = speed;
            this.layer = layer;
        }
    }
}
