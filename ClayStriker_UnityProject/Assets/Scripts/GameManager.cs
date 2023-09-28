using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Serializable]
    public class LevelManager
    {
        [SerializeField, Tooltip("Element 0 = Level Select, all others match their level order")]
        private string[] levelNames;

        public void LoadLevel(int num)
        {
            
            
            LoadScene(levelNames[num]);
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
    }
}
