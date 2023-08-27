using UnityEngine;

namespace Controllers.Player
{
    public class PlayerAnimator : MonoBehaviour
    {
        public static PlayerAnimator Instance;

        public Animator _animator;

        private string _currentState;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            _animator = GetComponent<Animator>();
        }

        // Update is called once per frame
        private void Update()
        {
        }
    }
}
