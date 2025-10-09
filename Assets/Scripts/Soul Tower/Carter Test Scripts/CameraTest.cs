using UnityEngine;

namespace SoulTower.Players
{
    public class CameraTest : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if(scrollInput > 0)
            {
                gameObject.transform.Translate(0f, .5f, 0f);
            } else if (scrollInput < 0)
            {
                gameObject.transform.Translate(0f, -.5f, 0f);
            }
        }
    }
}
