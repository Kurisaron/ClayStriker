using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class SaveManager : Singleton<SaveManager>
{
    private string saveFolderPath = Path.Combine(Application.dataPath, "Saves");
    private string SavePath { get => Path.Combine(saveFolderPath, "CS_Save.json"); }
    public SaveData saveData;

    public override void Awake()
    {
        base.Awake();

        saveData = ReadSaveFile();
        if (saveData == null) Debug.LogError("Still no save data");
    }

    public SaveData ReadSaveFile()
    {
        if (File.Exists(SavePath))
        {
            string contents = File.ReadAllText(SavePath);
            if (string.IsNullOrWhiteSpace(contents))
            {
                Debug.LogError("Save file contents were null");
                return null;
            }
            return JsonUtility.FromJson<SaveData>(contents);
        }
        else
        {
            saveData = new SaveData();
            WriteSaveFile();
            return ReadSaveFile();
        }
    }

    public void WriteSaveFile()
    {
        if (!Directory.Exists(saveFolderPath))
        {
            Debug.LogError("Directory does not exist, creating");
            Directory.CreateDirectory(saveFolderPath);
        }
        
        string contents = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(SavePath, contents);
    }

    public void NewScore(int levelNum, int score, out int newScoreIndex)
    {
        saveData.NewScore(levelNum, score, out newScoreIndex);
    }
}

[Serializable]
public class SaveData
{
    public LevelSave[] levelSaves;

    public SaveData()
    {
        levelSaves = new LevelSave[4];
        for (int i = 0; i < levelSaves.Length; ++i)
        {
            levelSaves[i] = new LevelSave(i + 1);
        }
    }

    public void NewScore(int level, int score, out int newScoreIndex) => levelSaves[level].NewScore(score, out newScoreIndex);
}

[Serializable]
public class LevelSave
{
    public int levelNum;
    public int[] leaderboard;

    public LevelSave(int level)
    {
        levelNum = level;
        leaderboard = new int[5];
        for (int i = 0; i < leaderboard.Length; ++i)
        {
            leaderboard[i] = 0;
        }
    }

    public void NewScore(int score, out int newScoreIndex)
    {
        int[] scores = new int[leaderboard.Length + 1];
        for (int i = 0; i < scores.Length; ++i)
        {
            if (i == scores.Length - 1)
            {
                scores[i] = score;
                continue;
            }
            
            scores[i] = leaderboard[i];
        }

        // Sort the scores to put them in ascending order then reverse them to put in descending (index of 0 is highest score)
        Array.Sort(scores);
        Array.Reverse(scores);

        for (int i = 0; i < leaderboard.Length; ++i)
        {
            leaderboard[i] = scores[i];
        }

        if (Array.Exists(leaderboard, element => element == score)) newScoreIndex = Array.FindIndex(leaderboard, element => element == score);
        else newScoreIndex = -1;
    }
}
