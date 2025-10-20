using UnityEngine;

namespace SoulTower.Enemies
{
    public readonly struct FixedSpeedAnimation : IEnemyAnimation
    {
        private readonly int animationID;
        private readonly float speed;
        private readonly int layer;

        public readonly int ID => animationID;
        public readonly float Speed => speed;
        public readonly int Layer => layer;

        public FixedSpeedAnimation(AnimationClip clip, float speed = 1.0f, int layer = 0)
        {
            animationID = Animator.StringToHash(clip.name);
            this.speed = speed;
            this.layer = layer;
        }

        public static implicit operator FixedSpeedAnimation(AnimationClip clip)
        {
            return new(clip);
        }
    }
}
