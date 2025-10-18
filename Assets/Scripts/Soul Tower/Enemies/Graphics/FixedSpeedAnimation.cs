using UnityEngine;

namespace SoulTower.Enemies
{
    public readonly struct FixedSpeedAnimation : IEnemyAnimation
    {
        private readonly int animationID;
        private readonly float speed;

        public readonly int ID => animationID;
        public readonly float Speed => speed;


        public FixedSpeedAnimation(string animationName, float speed)
        {
            animationID = Animator.StringToHash(animationName);
            this.speed = speed;
        }

        public FixedSpeedAnimation(AnimationClip clip)
        {
            animationID = Animator.StringToHash(clip.name);
            speed = 1.0f;
        }

        public FixedSpeedAnimation(AnimationClip clip, float speed)
        {
            animationID = Animator.StringToHash(clip.name);
            this.speed = speed;
        }

        public static implicit operator FixedSpeedAnimation(AnimationClip clip)
        {
            return new(clip);
        }
    }
}
