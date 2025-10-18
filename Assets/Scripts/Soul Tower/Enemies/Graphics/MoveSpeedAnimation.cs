using UnityEngine;

namespace SoulTower.Enemies
{
    public readonly struct MoveSpeedAnimation : IEnemyAnimation
    {
        private readonly Enemy enemy;
        private readonly int animationID;
        private readonly float speed;

        public readonly int ID => animationID;
        public readonly float Speed => speed * enemy.MoveSpeed * enemy.Model.WalkPlaybackSpeed;

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
    }
}
