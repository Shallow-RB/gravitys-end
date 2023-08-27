using System;
using TMPro;
using UI;
using UnityEngine;
using Utils;

public class GameOverScreen : MonoBehaviour
{
    [Header("Header")]
    [SerializeField]
    private TextMeshProUGUI header;

    [SerializeField]
    private string gameOverOnTime;

    [SerializeField]
    private string gameOverOnKIA;

    [Header("Game Stats")]
    [SerializeField]
    private TextMeshProUGUI objectivesCompleted;

    [SerializeField]
    private TextMeshProUGUI enemiesKilled;

    [SerializeField]
    private TextMeshProUGUI timePlayed;

    [SerializeField]
    private TextMeshProUGUI timeLeft;

    private void Start()
    {
        Navigation.instance.FadeIn();

        if (header != null) 
            header.text = GameStats.Instance.gameEnd == GameEnd.TIME ? gameOverOnTime : gameOverOnKIA;

        objectivesCompleted.text = GameStats.Instance.objectivesCompleted.ToString();
        enemiesKilled.text = GameStats.Instance.enemiesKilled.ToString();
        timePlayed.text = FormatTime(GameStats.Instance.timePlayed);
        timeLeft.text = FormatTime(GameStats.Instance.timeLeft);

        ResetStats();
    }

    private String FormatTime(float time)
    {
        float minutes = Mathf.FloorToInt(time / 60);
        float seconds = Mathf.FloorToInt(time % 60);

        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void ResetStats()
    {
        GameStats.Instance.objectivesCompleted = 0;
        GameStats.Instance.enemiesKilled = 0;
        GameStats.Instance.timePlayed = 0f;
        GameStats.Instance.timeLeft = 0f;
    }
}
