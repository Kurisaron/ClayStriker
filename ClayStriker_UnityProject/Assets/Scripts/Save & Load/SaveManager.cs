using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveManager : Singleton<SaveManager>
{
    private string savePath = Path.Combine(Application.dataPath, "Save", "CS_Save.json");
    [SerializeField] private SaveData saveData;

    public override void Awake()
    {
        base.Awake();

        saveData = ReadSaveFile();
        if (saveData == null) Debug.LogError("Still no save data");
    }

    public SaveData ReadSaveFile()
    {
        if (File.Exists(savePath))
        {
            string contents = File.ReadAllText(savePath);
            return JsonUtility.FromJson<SaveData>(contents);
        }
        else
        {
            if (saveData == null) Debug.LogError("No Save Data found at " + savePath);
            saveData = new SaveData();
            WriteSaveFile();
            return ReadSaveFile();
        }
    }

    public void WriteSaveFile()
    {
        string contents = JsonUtility.ToJson(saveData);
        File.WriteAllText(savePath, contents);
    }
}

[Serializable]
public class SaveData : ScriptableObject
{
    public int[] leaderboard = new int[5];

    public SaveData()
    {
        for (int i = 0; i < leaderboard.Length; ++i)
        {
            leaderboard[i] = 0;
        }
    }

    public void NewScore(int score)
    {

    }
}
