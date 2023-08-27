using UnityEngine;

namespace Core.Enemy
{
    public class BossManager : MonoBehaviour
    {
        public GameObject boss;

        #region Singleton

        public static BossManager Instance;

        private void Awake()
        {
            if (Instance != null)
                return;

            Instance = this;
        }

        #endregion
    }
}
