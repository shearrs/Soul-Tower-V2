using UnityEngine;

namespace SoulTower.Enemies
{
    public readonly struct SpeedAnimation
    {
        private readonly int animationID;
        private readonly float speed;

        public readonly int ID => animationID;
        public readonly float Speed => speed;

        public SpeedAnimation(int animationID, float speed)
        {
            this.animationID = animationID;
            this.speed = speed;
        }

        public SpeedAnimation(AnimationClip clip)
        {
            animationID = Animator.StringToHash(clip.name);
            speed = 1.0f;
        }

        public SpeedAnimation(AnimationClip clip, float speed)
        {
            animationID = Animator.StringToHash(clip.name);
            this.speed = speed;
        }

        public static implicit operator SpeedAnimation(AnimationClip clip)
        {
            return new(clip);
        }
    }
}
