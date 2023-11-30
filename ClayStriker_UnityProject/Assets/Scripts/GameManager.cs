using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public SceneLoader sceneLoader;
    
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
            UIManager.Instance.DisplayOptionsMenu(false);
        }
    }
    private string optionsContext = "";
    
    public override void Awake()
    {
        base.Awake();

        Score = 0;
        if (SceneManager.GetActiveScene().name == sceneLoader.loadingSceneName)
        {
            sceneLoader.LoadMainMenu();
            //InputEvents.Instance.SetInputState(InputState.Menu);
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
    public void MainMenuButton()
    {
        sceneLoader.LoadMainMenu();
    }

    public void LevelSelectButton()
    {
        sceneLoader.LoadLevel(0);
    }

    public void OptionsButton(string context)
    {
        optionsContext = context;

        if (context.Equals("MainMenu"))
        {
            UIManager.Instance.DisplayMainMenu(false);
            UIManager.Instance.DisplayOptionsMenu(true);
        }

        if (context.Equals("PauseMenu"))
        {
            UIManager.Instance.DisplayPauseScreen(false);
            UIManager.Instance.DisplayOptionsMenu(true);
        }
    }

    public void OptionsReturn()
    {
        if (optionsContext.Equals("MainMenu"))
        {
            UIManager.Instance.DisplayMainMenu(true);
            UIManager.Instance.DisplayOptionsMenu(false);
        }

        if (optionsContext.Equals("PauseMenu"))
        {
            UIManager.Instance.DisplayPauseScreen(true);
            UIManager.Instance.DisplayOptionsMenu(false);
        }

        SaveManager.Instance.WriteSaveFile();
    }

    public void NextLevelButton()
    {
        if (sceneLoader.GetLevelNum() >= sceneLoader.levelNames.Length - 1) sceneLoader.LoadCredits();
        else sceneLoader.LoadLevel(sceneLoader.GetLevelNum() + 1);
    }

    public void LevelButton(int level)
    {
        UIManager.Instance.DisplayLevelSelect(false);
        sceneLoader.LoadLevel(level);
    }



    public void QuitButton()
    {
        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #endif

        Application.Quit();
    }

    #endregion

    [Serializable]
    public class SceneLoader
    {
        public string loadingSceneName;
        public string mainMenuSceneName;
        public string creditsSceneName;
        [Tooltip("Element 0 = Level Select, all others match their level order")]
        public string[] levelNames;

        public void LoadMainMenu() => LoadScene(mainMenuSceneName, SceneLoadType.MainMenu);

        public void LoadLevel(int num)
        {
            if (num >= levelNames.Length)
            {
                Debug.LogWarning("Cannot load level " + num.ToString() + ", no level has been set");
                return;
            }

            bool isLoadingGameLevel = num > 0;

            LoadScene(levelNames[num], isLoadingGameLevel ? SceneLoadType.GameLevel : SceneLoadType.LevelSelect);

        }

        public void LoadCredits() => LoadScene(creditsSceneName, SceneLoadType.Credits);

        private void LoadReset(bool displayMainMenu, bool displayLevelSelect, bool displayGameScreen, bool displayCreditsScreen, InputState targetState)
        {
            UIManager.Instance.HideLeaderboard();
            GameManager.Instance.GamePaused = false;

            if (GetLevelNum() > 0)
            {
                Track track = Track.Instance;
                Destroy(track.player.gameObject);
                Destroy(track.gameObject);
            }

            UIManager.Instance.patController.PatWindowActive(false);
            UIManager.Instance.DisplayMainMenu(displayMainMenu);
            UIManager.Instance.DisplayLevelSelect(displayLevelSelect);
            UIManager.Instance.DisplayGameScreen(displayGameScreen);
            UIManager.Instance.DisplayCreditsScreen(displayCreditsScreen);
            InputEvents.Instance.SetInputState(targetState);

        }

        private void LoadScene(string levelName, SceneLoadType sceneLoadType)
        {
            (bool mainMenu, bool levelSelect, bool gameScreen, bool creditsScreen, InputState targetState) display = (false, false, false, false, InputState.Menu);
            switch (sceneLoadType)
            {
                case SceneLoadType.MainMenu:
                    display = (true, false, false, false, InputState.Menu);
                    break;
                case SceneLoadType.LevelSelect:
                    display = (false, true, false, false, InputState.Menu);
                    break;
                case SceneLoadType.GameLevel:
                    display = (false, false, true, false, InputState.Game);
                    break;
                case SceneLoadType.Credits:
                    display = (false, false, false, true, InputState.Menu);
                    break;
                default:
                    Debug.LogError("Somehow sceneLoadType did not match available values");
                    break;
            }

            //Debug.LogWarning("DISPLAY CONDITIONS FOUND");
            LoadReset(display.mainMenu, display.levelSelect, display.gameScreen, display.creditsScreen, display.targetState);
            //Debug.LogWarning("DISPLAY CONDITIONS SET");
            SceneManager.LoadScene(levelName, LoadSceneMode.Single);
        }

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

        public enum SceneLoadType
        {
            MainMenu,
            LevelSelect,
            GameLevel,
            Credits
        }
    }

}

