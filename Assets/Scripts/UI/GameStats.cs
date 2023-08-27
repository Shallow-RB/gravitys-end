using UnityEngine;

namespace UI
{
    public class GameStats : MonoBehaviour
    {
        public static GameStats Instance;

        public int objectivesCompleted;
        public int enemiesKilled;
        public float timePlayed;
        public float timeLeft;
        public GameEnd gameEnd;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
            // Check the PlayerPrefs value for VSync
            int vsyncValue = PlayerPrefs.GetInt("VSyncEnabled");

            // Enable or disable VSync based on the PlayerPrefs value
            QualitySettings.vSyncCount = vsyncValue;
        }
    }

    public enum GameEnd
    {
        TIME,
        KIA
    }
}