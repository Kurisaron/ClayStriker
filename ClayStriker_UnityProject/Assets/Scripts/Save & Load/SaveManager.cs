using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveManager : Singleton<SaveManager>
{
    
}

[Serializable]
public class SaveData
{
    public int[] leaderboard = new int[5];

    public void NewScore(int score)
    {

    }
}
