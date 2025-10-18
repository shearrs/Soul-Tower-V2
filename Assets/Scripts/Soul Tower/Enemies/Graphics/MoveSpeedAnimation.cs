using Shears.Logging;
using UnityEngine;

namespace SoulTower.Enemies
{
    public readonly struct MoveSpeedAnimation : IEnemyAnimation
    {
        private readonly Enemy enemy;
        private readonly int animationID;
        private readonly float speed;

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

                return speed * enemy.MoveSpeed * enemy.Model.WalkPlaybackSpeed;
            }
        }

        public MoveSpeedAnimation(Enemy enemy, string animName, float speed = 1.0f)
        {
            animationID = Animator.StringToHash(animName);
            this.enemy = enemy;
            this.speed = speed;
        }

        public MoveSpeedAnimation(Enemy enemy, AnimationClip clip, float speed = 1.0f)
        {
            animationID = Animator.StringToHash(clip.name);
            this.enemy = enemy;
            this.speed = speed;
        }

        public MoveSpeedAnimation(Enemy enemy, int id, float speed = 1.0f)
        {
            this.enemy = enemy;
            animationID = id;
            this.speed = speed;
        }
    }
}
