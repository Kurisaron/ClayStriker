using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private int score;
    
    public override void Awake()
    {
        base.Awake();

        score = 0;
    }

    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log("Score is now " + score.ToString());
    }
}
