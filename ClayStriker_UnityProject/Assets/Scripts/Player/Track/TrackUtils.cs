using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TrackUtils
{

    [MenuItem("Clay Strikers/Track/Start Track", priority = 1)]
    public static void StartTrack()
    {
        GameObject newTrack = new GameObject("Track");
        newTrack.transform.SetPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
        newTrack.AddComponent<Track>();
    }

    [MenuItem("Clay Strikers/Track/Start Track", true)]
    public static bool ValidateStartTrack()
    {
        return Object.FindObjectOfType<Track>() == null;
    }

    [MenuItem("Clay Strikers/Track/Add Stop", priority = 2)]
    public static void AddStop()
    {
        Track track = Object.FindObjectOfType<Track>();

        GameObject newStop = new GameObject("Stop" + track.stops.Count);
        newStop.transform.SetPositionRotationAndParent(Vector3.zero, Quaternion.Euler(Vector3.zero), track.transform);
        track.stops.Add(newStop.AddComponent<Stop>().Init(track));
    }

    [MenuItem("Clay Strikers/Track/Add Stop", true)]
    public static bool ValidateAddStop()
    {
        return Object.FindObjectOfType<Track>() != null;
    }

    [MenuItem("CONTEXT/Stop/Remove Stop")]
    public static void RemoveStop(Stop stop)
    {

    }
}
