using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public LevelManager levelManager;
    
    public GameObject playerPrefab;
    public GameObject bunkerPrefab;
    public int Score { get; private set; }

    private bool gamePaused = false;
    public bool GamePaused
    {
        get => gamePaused;
        set
        {
            gamePaused = value;
            Time.timeScale = value ? 0 : 1;
            InputEvents.Instance.SetInputState(value ? InputState.PauseMenu : InputState.Game);
            UIManager.Instance.DisplayPauseScreen(value);
        }
    }
    
    public override void Awake()
    {
        base.Awake();

        Score = 0;
        if (SceneManager.GetActiveScene().name == levelManager.loadingSceneName)
        {
            levelManager.LoadLevel(0);
            InputEvents.Instance.SetInputState(InputState.Menu);
        }
    }


    public void AddScore(int amount)
    {
        Score += amount;
        UIManager.Instance.UpdateScore();
        //Debug.Log("Score is now " + score.ToString());
    }

    public void ResetScore()
    {
        Score = 0;
        UIManager.Instance.UpdateScore();
    }


    #region Button Events

    public void LevelSelectButton()
    {
        levelManager.LoadLevel(0);
    }

    public void NextLevelButton()
    {
        if (levelManager.GetLevelNum() >= levelManager.levelNames.Length - 1) levelManager.LoadCredits();
        else levelManager.LoadLevel(levelManager.GetLevelNum() + 1);
    }

    public void LevelButton(int level)
    {
        UIManager.Instance.DisplayLevelSelect(false);
        levelManager.LoadLevel(level);
    }



    public void QuitButton()
    {
        

        Application.Quit();
    }

    #endregion

    [Serializable]
    public class LevelManager
    {
        public string loadingSceneName;
        public string creditsSceneName;
        [Tooltip("Element 0 = Level Select, all others match their level order")]
        public string[] levelNames;

        public void LoadLevel(int num)
        {
            if (num >= levelNames.Length)
            {
                Debug.LogWarning("Cannot load level " + num.ToString() + ", no level has been set");
                return;
            }

            bool isLoadingGameLevel = num > 0;
            LoadReset(isLoadingGameLevel, !isLoadingGameLevel, isLoadingGameLevel ? InputState.Game : InputState.Menu);

            LoadScene(levelNames[num]);

        }

        public void LoadCredits()
        {
            LoadReset(false, false, InputState.Menu);
            LoadScene(creditsSceneName);
        }

        private void LoadReset(bool displayGameScreen, bool displayLevelSelect, InputState targetState)
        {
            UIManager.Instance.HideLeaderboard();
            UIManager.Instance.DisplayGameScreen(displayGameScreen);
            GameManager.Instance.GamePaused = false;

            if (GetLevelNum() > 0)
            {
                Track track = Track.Instance;
                Destroy(track.player.gameObject);
                Destroy(track.gameObject);
            }

            UIManager.Instance.DisplayLevelSelect(displayLevelSelect);
            InputEvents.Instance.SetInputState(targetState);

        }

        private void LoadScene(string levelName) => SceneManager.LoadScene(levelName, LoadSceneMode.Single);

        public int GetLevelNum()
        {
            string num = "";
            string levelName = SceneManager.GetActiveScene().name;
            for (int i = 0; i < levelName.Length; ++i)
            {
                if (Char.IsDigit(levelName[i])) num += levelName[i];
            }
            if (num.Length > 0) return int.Parse(num);
            else return 0;
        }

        public bool IsLoadingScene()
        {
            return SceneManager.GetActiveScene().name == loadingSceneName;
        }

        public bool IsLevelSelectScene()
        {
            return SceneManager.GetActiveScene().name == levelNames[0];
        }
    }
}
