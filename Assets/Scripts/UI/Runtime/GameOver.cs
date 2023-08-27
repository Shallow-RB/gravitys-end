
using Core;
using UnityEngine;
using Utils;

namespace UI.Runtime
{
    public class GameOver : MonoBehaviour
    {
        public static GameOver Instance;

        private void Start()
        {
            if (Instance == null) Instance = this;
        }

        public void PlayerGameOver()
        {
            GameStats.Instance.timePlayed = Timer.instance.timePlayed;
            GameStats.Instance.timeLeft = Timer.instance.time;
            Timer.instance.timerIsRunning = false;
            GameStats.Instance.gameEnd = Timer.instance.time <= 0 ? GameEnd.TIME : GameEnd.KIA;
            Navigation.instance.GameOver();
        }

        public void PlayerEndGame()
        {
            Navigation.instance.EndGame();
        }
    }
}