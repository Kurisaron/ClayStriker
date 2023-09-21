using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Text scoreCounter;
    [SerializeField] private GameObject leaderboardScreenPrefab;

    private void Start()
    {
        UpdateScore();
    }

    public void UpdateScore()
    {
        scoreCounter.text = "Score: " + GameManager.Instance.Score.ToString();
    }

    public void DisplayLeaderboard()
    {

    }
}
