using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Stop : MonoBehaviour
{
    [SerializeField] private bool showDebug = true;
    
    public Track track;

    public List<Bunker> bunkers = new List<Bunker>();
    [SerializeField] private List<FiringStep> firingSequence = new List<FiringStep>();
    public List<Target> activeTargets = new List<Target>();

    [SerializeField] private float arrivalAngle = 0;
    public Vector3 ArrivalDirection
    {
        get
        {
            float radians = arrivalAngle.AngleToRadians();
            return new Vector3(Mathf.Sin(radians), 0, Mathf.Cos(radians));
        }
    }

    public Stop Init(Track t)
    {
        track = t;

        return this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.name.Contains("0") || other.transform.GetBaseTransform().gameObject.GetComponent<Player>() == null) return;

        Arrived();

        Player player = other.transform.GetBaseTransform().gameObject.GetComponent<Player>();

        player.SetBearing(new Func<Vector3>(() => ArrivalDirection));

        StartCoroutine(FiringSequenceRoutine());

        gameObject.GetComponent<SphereCollider>().enabled = false;
    }

    private IEnumerator FiringSequenceRoutine()
    {
        //Debug.LogError("Firing sequence started");
        for (int i = 0; i < firingSequence.Count; ++i)
        {
            FiringStep step = firingSequence[i];
            yield return new WaitForSeconds(step.delay);
            if (bunkers[step.bunkerAIndex] != null) activeTargets.Add(bunkers[step.bunkerAIndex].ShootTarget(this));
            if (bunkers[step.bunkerBIndex] != null) activeTargets.Add(bunkers[step.bunkerBIndex].ShootTarget(this));
        }

        //Debug.LogError("Firing sequence ended");
        yield return new WaitUntil(() => activeTargets.Count <= 0);

        //Debug.LogError("Passing to next stop");
        PassOn();
    }

    private void Arrived() => track.Arrived();

    private void PassOn() => track.PassOn();

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!showDebug) return;

        GUI.color = Color.white;
        for (int i = 0; i < bunkers.Count; ++i)
        {
            Handles.Label(bunkers[i].transform.position + Vector3.up, new GUIContent(i.ToString()));
            Gizmos.color = Color.red;
            Gizmos.DrawRay(bunkers[i].transform.position, bunkers[i].transform.forward * 3);
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, bunkers[i].transform.position);
        }
    }
    #endif

}

[Serializable]
public struct FiringStep
{
    [Min(0), Tooltip("Index of first bunker in list to fire")] public int bunkerAIndex;
    [Min(0), Tooltip("Index of second bunker in list to fire")] public int bunkerBIndex;
    [Min(0), Tooltip("Seconds of delay before this step is fired")] public float delay;
}
