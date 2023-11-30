using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;
using UnityEngine.UI;
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
        if (saveData == null)
        {
            Debug.LogError("Still no save data");
            return;
        }

        /*
        foreach (LevelSave levelSave in saveData.levelSaves)
        {
            Debug.Log(levelSave.PrintScores());
        }
        */
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
            SaveData saveData = JsonUtility.FromJson<SaveData>(contents);
            // Set in-game functions to reflect saved option values
            saveData.options.Resolution = saveData.options.Resolution;
            saveData.options.SensitivityLevel = saveData.options.SensitivityLevel;
            saveData.options.AudioLevel = saveData.options.AudioLevel;
            saveData.options.CrosshairActive = saveData.options.CrosshairActive;
            return saveData;
        }
        else
        {
            saveData = new SaveData(OptionsSave.ResolutionSave.GetResolution(ResolutionOption.DetectDisplay));
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

        if (levelIndex + 1 >= saveData.levelSaves.Length) return;
        LevelSave nextLevel = saveData.levelSaves[levelIndex + 1];
        if (score > 0 && nextLevel != null && !nextLevel.levelUnlocked && newScoreIndex >= 0) nextLevel.levelUnlocked = true;
    }

    public void ResolutionDropdown(Dropdown dropdown)
    {
        saveData.options.Resolution = OptionsSave.ResolutionSave.GetResolution((ResolutionOption)dropdown.value);
    }

    public void SensitivitySlider(Slider slider)
    {
        saveData.options.SensitivityLevel = slider.value;
    }

    public void AudioSlider(Slider slider)
    {
        saveData.options.AudioLevel = slider.value;
    }

    public void CrosshairToggle(Toggle toggle)
    {
        saveData.options.CrosshairActive = toggle.isOn;
    }
}

[Serializable]
public class SaveData
{
    public OptionsSave options;
    public LevelSave[] levelSaves;

    public SaveData(OptionsSave.ResolutionSave resolutionSave)
    {
        options = new OptionsSave(resolutionSave);
        levelSaves = new LevelSave[2];
        for (int i = 0; i < levelSaves.Length; ++i)
        {
            levelSaves[i] = new LevelSave(i + 1);
        }
    }

    public void NewScore(int level, int score, out int newScoreIndex) => levelSaves[level].NewScore(score, out newScoreIndex);
}

[Serializable]
public class OptionsSave
{
    [SerializeField] private ResolutionSave resolution;
    public ResolutionSave Resolution
    {
        get => resolution;
        set
        {
            resolution = value;
            Screen.SetResolution(value.width, value.height, Screen.fullScreenMode);
            Debug.Log("Screen Resolution is now " + value.width.ToString() + " x " + value.height.ToString());
        }
    }
    [SerializeField] private float sensitivityLevel;
    public float SensitivityLevel
    {
        get => sensitivityLevel;
        set
        {
            sensitivityLevel = value;
            UIManager.Instance.cameraSettings.turnSpeed = Mathf.Lerp(0.0f, 0.5f, value);
        }
    }
    [SerializeField] private float audioLevel;
    public float AudioLevel
    {
        get => audioLevel;
        set
        {
            audioLevel = value;
            Debug.Log("Audio level set to " + (value * 100.0f).ToString() + " percent");
        }
    }
    [SerializeField] private bool crosshairActive;
    public bool CrosshairActive
    {
        get => crosshairActive;
        set
        {
            crosshairActive = value;
            UIManager.Instance.DisplayCrosshair(value);
        }
    }

    public OptionsSave(ResolutionSave resolutionSave)
    {
        resolution = resolutionSave;
        sensitivityLevel = 0.5f;
        audioLevel = 1.0f;
        crosshairActive = true;
    }

    [Serializable] public class ResolutionSave
    {
        public int width;
        public int height;

        public static ResolutionSave GetResolution(ResolutionOption resolutionOption)
        {
            switch (resolutionOption)
            {
                case ResolutionOption._1920x1080:
                    return new ResolutionSave() { width = 1920, height = 1080 };
                case ResolutionOption._1600x900:
                    return new ResolutionSave() { width = 1600, height = 900 };
                case ResolutionOption._1366x768:
                    return new ResolutionSave() { width = 1366, height = 768 };
                case ResolutionOption._1200x1024:
                    return new ResolutionSave() { width = 1200, height = 1024 };
                case ResolutionOption.DetectDisplay:
                    return new ResolutionSave() { width = Display.main.systemWidth, height = Display.main.systemHeight };
                default:
                    Debug.LogError("Resolution option passed is invalid, option: " + resolutionOption.ToString());
                    return GetResolution(ResolutionOption.DetectDisplay);
            }
        }
    }
}

[Serializable]
public enum ResolutionOption
{
    _1920x1080,
    _1600x900,
    _1366x768,
    _1200x1024,
    DetectDisplay
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
