using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Endless : Singleton<Endless>
{
    [HideInInspector] public Player player;

    private int targetsMissed;
    public int TargetsMissed
    {
        get => targetsMissed;
        set
        {
            targetsMissed = value;
        }
    }

    [SerializeField] private List<BunkerPair> bunkerPairs;

    private void Start()
    {
        GameObject playerObject = Instantiate(GameManager.Instance.playerPrefab);
        playerObject.transform.SetPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
        player = playerObject.GetComponent<Player>();
        player.SetBearing(() => transform.forward);

        targetsMissed = 0;
        StartCoroutine(EndlessModeRoutine());
    }

    private IEnumerator EndlessModeRoutine()
    {
        while (targetsMissed < 3)
        {
            if (bunkerPairs == null || bunkerPairs.Count <= 0)
            {
                Debug.LogError("No bunker pairs");
                break;
            }

            BunkerPair bunkerPair = bunkerPairs[UnityEngine.Random.Range(0, bunkerPairs.Count)];
            if (bunkerPair.bunkerA != null) bunkerPair.bunkerA.ShootTarget();
            if (bunkerPair.bunkerB != null && bunkerPair.bunkerA != bunkerPair.bunkerB) bunkerPair.bunkerB.ShootTarget();

            yield return new WaitForSeconds(UnityEngine.Random.Range(0.5f, 2.0f));
        }

        End();
    }

    private void End()
    {
        InputEvents.Instance.SetInputState(InputState.Menu);

        int levelNum = GameManager.Instance.sceneLoader.GetLevelNum();
        SaveManager.Instance.NewScore(levelNum - 1, GameManager.Instance.Score, out int newScoreIndex);
        SaveManager.Instance.WriteSaveFile();
        GameManager.Instance.ResetScore();

        UIManager.Instance.DisplayLeaderboard(levelNum, newScoreIndex);
    }
}

[Serializable]
public class BunkerPair
{
    public Bunker bunkerA;
    public Bunker bunkerB;
}
