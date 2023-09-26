using System.Collections;
using System.Collections.Generic;
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
        levelManager = new LevelManager();
    }

    public void AddScore(int amount)
    {
        Score += amount;
        UIManager.Instance.UpdateScore();
        //Debug.Log("Score is now " + score.ToString());
    }


    public class LevelManager
    {
        public LevelManager()
        {
            
        }

        public void LoadLevel(string levelName) => SceneManager.LoadScene(levelName, LoadSceneMode.Single);
    }
}
