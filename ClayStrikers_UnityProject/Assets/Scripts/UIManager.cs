using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private Text scoreCounter;
    [SerializeField] private GameObject leaderboardScreen;
    [SerializeField] private GameObject levelSelectScreen;
    

    public void UpdateScore()
    {
        if (scoreCounter.gameObject.activeInHierarchy) scoreCounter.text = "Score: " + GameManager.Instance.Score.ToString();
    }

    public void DisplayScore(bool active)
    {
        scoreCounter.gameObject.SetActive(active);
    }

    public void DisplayLeaderboard(int level, int newScoreIndex)
    {
        leaderboardScreen.SetActive(true);

        GameObject[] scoreText = new GameObject[5];
        for (int i = 0; i < scoreText.Length; ++i)
        {
            scoreText[i] = leaderboardScreen.transform.Find("Score " + (i + 1).ToString()).gameObject;
            scoreText[i].GetComponent<Image>().color = Color.grey;
        }

        LevelSave levelSave = Array.Find(SaveManager.Instance.saveData.levelSaves, save => save.levelNum == level);
        for (int i = 0; i < levelSave.leaderboard.Length; ++i)
        {
            string scoreDesignator = i == newScoreIndex ? "NEW SCORE: " : GetPlace(i) + " Place: ";
            if (i == newScoreIndex) scoreText[i].GetComponent<Image>().color = Color.yellow;
            scoreText[i].GetComponentInChildren<Text>().text = scoreDesignator + levelSave.leaderboard[i].ToString();

        }

        string GetPlace(int num)
        {
            switch (num)
            {
                case 0:
                    return "1st";
                case 1:
                    return "2nd";
                case 2:
                    return "3rd";
                case 3:
                    return "4th";
                case 4:
                    return "5th";
                default:
                    return "ERROR";
            }
        }
    }

    public void HideLeaderboard()
    {
        leaderboardScreen.SetActive(false);
    }

    public void DisplayLevelSelect(bool active)
    {
        levelSelectScreen.SetActive(active);
    }
}
