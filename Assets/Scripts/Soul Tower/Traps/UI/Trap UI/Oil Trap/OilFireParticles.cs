using Shears;
using System.Collections;
using UnityEngine;

namespace SoulTower.Traps.UI
{
    public class OilFireParticles : MonoBehaviour
    {
        void Start()
        {
            StartCoroutine(DelayedDestroy());
        }

        private IEnumerator DelayedDestroy()
        {
            yield return CoroutineUtil.WaitForSeconds(7f);
            Destroy(gameObject);
        }
    }
}
