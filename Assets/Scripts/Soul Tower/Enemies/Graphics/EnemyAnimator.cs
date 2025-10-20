using System.Collections;
using UnityEngine;

namespace SoulTower.Enemies
{
    public class EnemyAnimator : MonoBehaviour
    {
        private static readonly int BLEND_PARAMETER = Animator.StringToHash("blend");

        [SerializeField] private Animator animator;

        private MoveSpeedAnimation moveSpeedAnim;
        private Coroutine moveSpeedCoroutine;

        public bool IsInAnimation(IEnemyAnimation anim)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).shortNameHash == anim.ID && !animator.IsInTransition(0))
                return true;
            else
                return false;
        }

        public void CrossFade(IEnemyAnimation anim, float fadeDuration, int layer)
        {
            if (IsInAnimation(anim))
                return;

            animator.CrossFade(anim.ID, fadeDuration, layer);

            if (anim is MoveSpeedAnimation moveAnim)
            {
                moveSpeedAnim = moveAnim;
                StopAllCoroutines();
                moveSpeedCoroutine = StartCoroutine(IEUpdateWithMoveSpeed());
            }
            else if (moveSpeedCoroutine != null)
            {
                StopCoroutine(moveSpeedCoroutine);
                moveSpeedCoroutine = null;
            }
        }

        public void SetAnimationSpeed(float speed)
        {
            animator.speed = speed;
        }

        public void SetBlend(float value)
        {
            animator.SetFloat(BLEND_PARAMETER, value);
        }

        private IEnumerator IEUpdateWithMoveSpeed()
        {
            while (true)
            {
                animator.speed = moveSpeedAnim.Speed;

                yield return null;
            }
        }
    }
}
