using UnityEngine;
using TMPro;
using Core.Enemy;
using UI.Runtime;
using UI.Tokens;
using Core.Audio;
using Utils;
using UI;

namespace Core
{
    public class Timer : MonoBehaviour
    {
        [SerializeField]
        public float time = 1800;

        [SerializeField]
        public float dangerZone = 600;

        [SerializeField]
        private float timePerKill = 0.02f;

        [SerializeField]
        public TextMeshProUGUI display;

        public static Timer instance;
        public float startingTime { get; set; }
        public float timePlayed { get; set; }

        private bool _timerIsRunning;
        public bool timerIsRunning
        {
            get => _timerIsRunning;
            set
            {
                _timerIsRunning = value;
                OnTimerIsRunning?.Invoke(timerIsRunning);
            }
        }

        public delegate void DialogueActive(bool mapOpened);
        public static event DialogueActive OnTimerIsRunning;

        private bool isPlayingClockSound;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
        }

        private void Start()
        {
            StartTimer();
            DisplayTime(time);

            EnemyBase.OnEnemyKilled += AddEnemyTime;
        }

        private void Update()
        {
            // Checks whether the timer update should be executed
            if (timerIsRunning && !Navigation.instance.loadingScreenActive)
            {
                if (time > 0)
                {
                    time -= Time.deltaTime;
                    timePlayed += Time.deltaTime;
                    GameStats.Instance.timePlayed = timePlayed;
                    GameStats.Instance.timeLeft = time;
                }
                else
                {
                    time = 0;
                    timerIsRunning = false;
                    GameOver.Instance.PlayerGameOver();
                }
                DisplayTime(time);
            }
        }

        private void StartTimer()
        {
            // Starts the timer
            timerIsRunning = true;
            startingTime = time;
        }

        private void AddEnemyTime()
        {
            time += timePerKill * TokenManager.instance.timeSection.GetModifier();
        }

        private void DisplayTime(float ttd)
        {
            float minutes = Mathf.FloorToInt(ttd / 60);
            float seconds = Mathf.FloorToInt(ttd % 60);
            float milliseconds = Mathf.FloorToInt((ttd * 1000) % 1000);

            if (ttd <= dangerZone && !isPlayingClockSound)
            {
                display.color = Color.red;
                // Play clock ticking sound effect
                isPlayingClockSound = true;
                SoundEffectsManager.instance.PlaySoundEffect(SoundEffect.CLOCK_TICKING);
            }

            display.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
        }
    }
}
