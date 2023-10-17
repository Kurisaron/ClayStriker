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

    public void OnEnable()
    {
        
        saveData = ReadSaveFile();
        if (saveData == null) Debug.LogError("Still no save data");

        foreach (LevelSave levelSave in saveData.levelSaves)
        {
            Debug.Log(levelSave.PrintScores());
        }
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

    public void NewScore(int levelIndex, int score, out int newScoreIndex)
    {
        saveData.NewScore(levelIndex, score, out newScoreIndex);

        LevelSave nextLevel = saveData.levelSaves[levelIndex + 1];
        if (!nextLevel.levelUnlocked && newScoreIndex >= 0) nextLevel.levelUnlocked = true;
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
    public bool levelUnlocked;
    public int[] leaderboard;

    public LevelSave(int level)
    {
        levelNum = level;
        levelUnlocked = level == 1;
        leaderboard = new int[5];
        for (int i = 0; i < leaderboard.Length; ++i)
        {
            leaderboard[i] = 0;
        }
    }

    public void NewScore(int score, out int newScoreIndex)
    {
        List<int> checkScores = new List<int>();
        checkScores.Add(score);
        for (int i = 0; i < leaderboard.Length; ++i)
        {
            checkScores.Add(leaderboard[i]);
        }
        checkScores.Sort();
        checkScores.Reverse();
        for (int i = 0; i < leaderboard.Length; ++i)
        {
            Debug.Log(checkScores[i].ToString());
            leaderboard[i] = checkScores[i];
        }
        Debug.Log(checkScores[leaderboard.Length].ToString());

        if (Array.Exists(leaderboard, element => element == score)) newScoreIndex = Array.FindIndex(leaderboard, element => element == score);
        else newScoreIndex = -1;
    }

    public string PrintScores()
    {
        string output = "( ";
        for (int i = 0; i < leaderboard.Length; ++i)
        {
            output += leaderboard[i] + (i != leaderboard.Length - 1 ? ", " : " )");
        }
        return output;
    }
}