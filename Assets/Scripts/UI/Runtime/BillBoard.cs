using UnityEngine;

namespace UI.Runtime
{
    public class BillBoard : MonoBehaviour
    {
        private Transform _cam;

        private void Start()
        {
            _cam = GameObject.FindWithTag("MainCamera").transform;
        }

        private void LateUpdate()
        {
            transform.LookAt(transform.position + _cam.forward);
        }
    }
}
