using UnityEngine;

namespace Utils
{
    public class LockRotation : MonoBehaviour
    {
        private Quaternion _lockedRotation;

        private void Start()
        {
            _lockedRotation = transform.rotation;
        }

        private void Update()
        {
            transform.rotation = _lockedRotation;
        }
    }
}
