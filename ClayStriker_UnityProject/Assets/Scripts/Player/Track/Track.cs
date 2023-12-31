using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

public class Track : GameMode<Track>
{
    [SerializeField] private bool showDebug = true;

    public Player player;

    private float playerMoveSpeed = 10.0f;

    // Set of stops for player to stop at
    public List<Stop> stops = new List<Stop>();

    private Queue<Stop> route = new Queue<Stop>();
    private Stop currentStop = null;
    private Stop targetStop = null;

    public override void Awake()
    {
        base.Awake();
        targetStatus = (0, 0);
    }

    private void Start()
    {

        StartRoute();
    }

    private void Update()
    {
        if (currentStop != null && targetStop != null && currentStop != targetStop) MoveAlongRoute();
    }

    private void StartRoute()
    {

        if (stops.Count == 0)
        {
            Debug.LogError("No stops in track, can't start route");
            return;
        }

        route = new Queue<Stop>();
        for (int i = 0; i < stops.Count; ++i)
        {
            route.Enqueue(stops[i]);
        }
        currentStop = route.Dequeue();

        GameObject newPlayer = Instantiate(GameManager.Instance.playerPrefab);
        newPlayer.transform.position = currentStop.transform.position;
        player = newPlayer.GetComponent<Player>();
        player.SetBearing(new Func<Vector3>(() => currentStop.ArrivalDirection));
        if (currentStop.PatDepartureDialogue != PatDialogueContext.None) UIManager.Instance.patController.DisplayDialogue(currentStop.PatDepartureDialogue);

        if (route.Count > 0) targetStop = route.Dequeue();
        else Debug.LogError("No stops to put as first target");
    }

    private void MoveAlongRoute()
    {
        player.gameObject.transform.position = Vector3.MoveTowards(player.gameObject.transform.position, targetStop.transform.position, playerMoveSpeed * Time.deltaTime);
    }

    public void Arrived()
    {
        currentStop = targetStop;
    }

    public async void PassOn()
    {
        if (route.Count > 0) targetStop = route.Dequeue();
        else await End();
    }

    public override async Task End()
    {
        await base.End();

    }

    private void DisplayLeaderboard(int levelNum, int newScoreIndex)
    {
        UIManager.Instance.DisplayLeaderboard(levelNum, newScoreIndex);
    }

    public void OnDrawGizmos()
    {
        if (!showDebug) return;
        
        Gizmos.color = Color.yellow;
        Vector3[] stopPositions = stops.ConvertAll(stop => stop.gameObject.transform.position).ToArray();
        Gizmos.DrawLineStrip(stopPositions, false);

        for (int i = 0; i < stopPositions.Length; ++i)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(stopPositions[i], stops[i].ArrivalDirection.normalized * 3);
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(stopPositions[i], 0.3f);
        }
    }
}
