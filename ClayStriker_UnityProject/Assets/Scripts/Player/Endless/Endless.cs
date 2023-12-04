using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Endless : GameMode<Endless>
{
    [HideInInspector] public Player player;

    public int TargetsMissed
    {
        get => targetStatus.missed;
        set
        {
            targetStatus.missed = value;
        }
    }

    [SerializeField] private List<BunkerPair> bunkerPairs;

    private void Start()
    {
        GameObject playerObject = Instantiate(GameManager.Instance.playerPrefab);
        playerObject.transform.SetPositionAndRotation(transform.position, Quaternion.Euler(Vector3.zero));
        player = playerObject.GetComponent<Player>();
        player.SetBearing(() => transform.forward);

        StartCoroutine(EndlessModeRoutine());
    }

    private IEnumerator EndlessModeRoutine()
    {
        while (targetStatus.missed < 3)
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

    public override async Task End()
    {
        await base.End();

    }
}

[Serializable]
public class BunkerPair
{
    public Bunker bunkerA;
    public Bunker bunkerB;
}
