using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameMode<T> : Singleton<T> where T : Component
{

    public (int missed, int hit) targetStatus;

    public override void Awake()
    {
        base.Awake();

        targetStatus = (0, 0);
    }

    public virtual async Task End()
    {
        InputEvents.Instance.SetInputState(InputState.Menu);

        int levelNum = GameManager.Instance.sceneLoader.GetLevelNum();
        SaveManager.Instance.NewScore(levelNum - 1, GameManager.Instance.Score, out int newScoreIndex);
        SaveManager.Instance.WriteSaveFile();
        GameManager.Instance.ResetScore();

        UIManager.Instance.patController.DisplayDialogue(targetStatus.hit > 11 ? (targetStatus.missed == 0 ? PatDialogueContext.CriticalPass : PatDialogueContext.Pass) : (targetStatus.hit > 0 ? PatDialogueContext.Fail : PatDialogueContext.CriticalFail));
        await UIManager.Instance.patController.WaitForStopDisplay();

        UIManager.Instance.DisplayLeaderboard(levelNum, newScoreIndex);
    }
}
