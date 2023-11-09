using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static GameManager;

public class UIManager : Singleton<UIManager>
{
    // SCREENS
    [SerializeField] private GameObject gameScreen;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject leaderboardScreen;
    [SerializeField] private GameObject mainMenuScreen;
    [SerializeField] private GameObject levelSelectScreen;
    [SerializeField] private GameObject optionsScreen;
    [SerializeField] private GameObject creditsScreen;

    // UI PARTS
    [SerializeField] private GameObject crosshair;
    [SerializeField] private Text scoreCounter;
    [SerializeField] private Text nextLevelButtonText;
    [SerializeField] private GameObject level2Button;

    // PAT DIALOGUE
    public PatController patController;

    public void DisplayMainMenu(bool active)
    {
        mainMenuScreen.SetActive(active);
    }

    public void UpdateScore()
    {
        if (scoreCounter.gameObject.activeInHierarchy) scoreCounter.text = "Score: " + GameManager.Instance.Score.ToString();
    }

    public void DisplayCrosshair(bool active)
    {
        crosshair.SetActive(active);
    }

    public void DisplayGameScreen(bool active)
    {
        gameScreen.SetActive(active);
    }

    public void DisplayPauseScreen(bool active)
    {
        pauseScreen.SetActive(active);
    }

    public void DisplayCreditsScreen(bool active)
    {
        creditsScreen.SetActive(active);
    }

    public void DisplayLeaderboard(int level, int newScoreIndex)
    {
        leaderboardScreen.SetActive(true);

        nextLevelButtonText.text = GameManager.Instance.sceneLoader.GetLevelNum() >= GameManager.Instance.sceneLoader.levelNames.Length - 1 ? "Credits" : "Next Level";

        GameObject[] scoreText = new GameObject[5];
        for (int i = 0; i < scoreText.Length; ++i)
        {
            scoreText[i] = leaderboardScreen.transform.Find("Score " + (i + 1).ToString()).gameObject;
            scoreText[i].GetComponent<Image>().color = Color.grey;
        }

        LevelSave levelSave = Array.Find(SaveManager.Instance.saveData.levelSaves, save => save.levelNum == level);
        for (int i = 0; i < levelSave.leaderboard.Length; ++i)
        {
            string scoreDesignator = i == newScoreIndex ? "NEW SCORE: " : GetPlace(i) + " Place: ";
            if (i == newScoreIndex) scoreText[i].GetComponent<Image>().color = Color.yellow;
            scoreText[i].GetComponentInChildren<Text>().text = scoreDesignator + levelSave.leaderboard[i].ToString();

        }

        string GetPlace(int num)
        {
            switch (num)
            {
                case 0:
                    return "1st";
                case 1:
                    return "2nd";
                case 2:
                    return "3rd";
                case 3:
                    return "4th";
                case 4:
                    return "5th";
                default:
                    return "ERROR";
            }
        }
    }

    public void HideLeaderboard()
    {
        leaderboardScreen.SetActive(false);
    }

    public void DisplayLevelSelect(bool active)
    {
        levelSelectScreen.SetActive(active);

        level2Button.SetActive(active && SaveManager.Instance.saveData.levelSaves[1].levelUnlocked);
    }

    public void DisplayOptionsMenu(bool active)
    {
        optionsScreen.SetActive(active);
    }
}

[Serializable]
public class PatController
{
    [SerializeField] private GameObject patWindow;
    [SerializeField] private Image patFace;
    [SerializeField] private Text patSpeech;

    [SerializeField] private List<PatFace> patFaces;

    [SerializeField] private List<PatDialogue> dialogues;
    private PatDialoguePart[] activeDialogue;

    public async void DisplayDialogue(PatDialogueContext dialogueContext)
    {
        PatDialogue dialogue = dialogues.Find(check => check.Context == dialogueContext);
        if (dialogue.Context == PatDialogueContext.None) return;
        if (dialogue == null)
        {
            Debug.LogError("No dialogue matching context");
            return;
        }
        activeDialogue = dialogue;

        PatWindowActive(true);
        for (int i = 0; i < activeDialogue.Length; ++i)
        {
            SetPatDialogue(activeDialogue[i].Face, activeDialogue[i].Speech);
            await Task.Delay(TimeSpan.FromSeconds(3.0));
        }
        PatWindowActive(false);
    }

    private void PatWindowActive(bool active) => patWindow.SetActive(active);

    private void SetPatDialogue(PatFaceEnum face, string speech)
    {
        Sprite sprite = patFaces.Find(patFace => patFace.faceEnum == face).faceSprite;
        if (sprite != null) patFace.sprite = sprite;
        else Debug.LogWarning("No face available for current dialogue.");
        patSpeech.text = speech;
    }
}

[Serializable]
public class PatDialogue
{
    [SerializeField] private PatDialogueContext context;
    public PatDialogueContext Context { get => context; }
    [SerializeField] private PatDialoguePart[] parts;

    public static implicit operator PatDialoguePart[](PatDialogue patDialogue) => patDialogue.parts;
}

[Serializable]
public class PatDialoguePart
{
    [SerializeField] private PatFaceEnum face;
    public PatFaceEnum Face { get => face; }
    [SerializeField] private string speech;
    public string Speech { get => speech; }
}

[Serializable]
public class PatFace
{
    public PatFaceEnum faceEnum;
    public Sprite faceSprite;
}

public enum PatDialogueContext
{
    None,
    Level1_Start,
    Level1_AfterStation1,
    Level1_AfterStation2,
}

public enum PatFaceEnum
{
    Neutral,
    NeutralTalking,
    Happy,
    HappyTalking,
    HappyExtreme,
    Disappointed,
    DisappointedTalking,
    DisappointedExtreme,
    DisappointedExtremeTalking
}
