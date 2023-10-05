using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    public LevelManager levelManager;
    
    public GameObject playerPrefab;
    public GameObject bunkerPrefab;
    public int Score { get; private set; }
    
    public override void Awake()
    {
        base.Awake();

        Score = 0;
        if (SceneManager.GetActiveScene().name == levelManager.loadingSceneName) levelManager.LoadLevel(0);
        InputEvents.Instance.SetInputState(InputState.Game);
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

    public void LevelSelectButton()
    {
        levelManager.LoadLevel(0);
    }

    public void Level1Button()
    {
        UIManager.Instance.DisplayLevelSelect(false);
        levelManager.LoadLevel(1);
    }

    public void QuitButton()
    {
        

        Application.Quit();
    }

    [Serializable]
    public class LevelManager
    {
        public string loadingSceneName;
        [SerializeField, Tooltip("Element 0 = Level Select, all others match their level order")]
        private string[] levelNames;

        public void LoadLevel(int num)
        {
            UIManager.Instance.HideLeaderboard();
            UIManager.Instance.DisplayScore(num != 0);
            
            if (GetLevelNum() > 0)
            {
                Track track = Track.Instance;
                Destroy(track.player.gameObject);
                Destroy(track.gameObject);
            }

            LoadScene(levelNames[num]);

            if (num == 0) UIManager.Instance.DisplayLevelSelect(true);
            InputEvents.Instance.SetInputState(num > 0 ? InputState.Game : InputState.Menu);
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
    }
}
