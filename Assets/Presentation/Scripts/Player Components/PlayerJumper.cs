using System.Collections;
using UnityEngine;

namespace Presentation
{
    public class PlayerJumper : MonoBehaviour
    {
        public PlayerFlags flags;
        public CharacterController controller;

        public void Jump(float jumpForce, float gravity)
        {
            StartCoroutine(IEJump(jumpForce, gravity));
        }

        private IEnumerator IEJump(float jumpForce, float gravity)
        {
            flags.isJumping = true;

            float verticalForce = jumpForce;

            while (verticalForce > 0)
            {
                controller.Move(new Vector3(0, verticalForce * Time.deltaTime, 0));
                verticalForce += gravity * Time.deltaTime;

                yield return null;
            }

            flags.isJumping = false;
        }
    }
}
