using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TrackUtils
{
    #region Menu Buttons
    [MenuItem("GameObject/Clay Strikers/Track/Start Track")]
    public static void StartTrack(MenuCommand menuCommand)
    {
        GameObject newTrack = new GameObject("Track");
        newTrack.transform.SetPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
        newTrack.AddComponent<Track>();
    }

    [MenuItem("GameObject/Clay Strikers/Track/Start Track", true)]
    public static bool ValidateStartTrack()
    {
        return UnityEngine.Object.FindObjectOfType<Track>() == null;
    }

    [MenuItem("GameObject/Clay Strikers/Track/Add Stop")]
    public static void ContextAddStop(MenuCommand menuCommand)
    {
        AddStop(Selection.activeGameObject.GetComponent<Track>());
    }

    [MenuItem("GameObject/Clay Strikers/Track/Add Stop", true)]
    public static bool ValidateContextAddStop()
    {
        return Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Track>() != null;
    }

    [MenuItem("GameObject/Clay Strikers/Track/Remove Stop")]
    public static void ContextRemoveStop(MenuCommand menuCommand)
    {
        RemoveStop(Selection.activeGameObject.GetComponent<Stop>());

    }

    [MenuItem("GameObject/Clay Strikers/Track/Remove Stop", true)]
    public static bool ValidateContextRemoveStop()
    {
        return Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Stop>() != null;
    }

    [MenuItem("GameObject/Clay Strikers/Track/Switch Stops")]
    public static void ContextSwitchStops(MenuCommand menuCommand)
    {
        if (menuCommand.context != Selection.objects[0]) return;

        SwitchStops();
    }

    [MenuItem("GameObject/Clay Strikers/Track/Switch Stops", true)]
    public static bool ValidateContextSwitchStop()
    {
        return !ValidateStartTrack() && Selection.count == 2 && Selection.objects[0] is GameObject && Selection.objects[1] is GameObject && (Selection.objects[0] as GameObject).GetComponent<Stop>() != null && (Selection.objects[1] as GameObject).GetComponent<Stop>() != null;
    }

    [MenuItem("GameObject/Clay Strikers/Track/Add Bunker")]
    public static void ContextAddBunker(MenuCommand menuCommand)
    {
        AddBunker(Selection.activeGameObject.GetComponent<Stop>());
    }

    [MenuItem("GameObject/Clay Strikers/Track/Add Bunker", true)]
    public static bool ValidateContextAddBunker()
    {
        return Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Stop>() != null;
    }

    [MenuItem("GameObject/Clay Strikers/Track/Remove Bunker")]
    public static void ContextRemoveBunker(MenuCommand menuCommand)
    {
        RemoveBunker(Selection.activeGameObject.GetComponent<Bunker>());
    }

    [MenuItem("GameObject/Clay Strikers/Track/Remove Bunker", true)]
    public static bool ValidateContextRemoveBunker()
    {
        return Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Bunker>() != null;
    }

    #endregion

    #region Misc Utility
    public static void AddStop(Track track)
    {
        GameObject newStop = new GameObject("Stop" + track.stops.Count);
        Vector3 position = track.stops.Count == 0 ? track.transform.position : track.stops[^1].transform.position;
        newStop.transform.SetPositionRotationAndParent(position, Quaternion.Euler(Vector3.zero), track.transform);
        SphereCollider collider = newStop.AddComponent<SphereCollider>();
        collider.isTrigger = true;
        collider.radius = 0.3f;
        track.stops.Add(newStop.AddComponent<Stop>().Init(track));
    }

    public static void RemoveStop(Stop stop)
    {
        Track track = stop.track;
        track.stops.Remove(stop);

        FixStopNames(track);

        GameObject.DestroyImmediate(stop.gameObject);
    }

    public static void SwitchStops()
    {
        GameObject[] stops = new GameObject[2];
        for (int i = 0; i < stops.Length; ++i)
        {
            stops[i] = Selection.objects[i] as GameObject;
        }
        Track track = UnityEngine.Object.FindObjectOfType<Track>();
        track.stops.Switch(stops[0].GetComponent<Stop>(), stops[1].GetComponent<Stop>());
        FixStopNames(track);
    }

    private static void FixStopNames(Track track)
    {
        for (int i = 0; i < track.stops.Count; ++i)
        {
            Stop stop = track.stops[i];
            stop.gameObject.name = "Stop" + i.ToString();
        }
    }

    public static void AddBunker(Stop stop)
    {
        GameObject newBunker = GameObject.Instantiate(GameManager.Instance.bunkerPrefab);
        newBunker.name = "Bunker(" + stop.gameObject.name + ")";
        newBunker.transform.SetPositionAndRotation(stop.transform.position, Quaternion.Euler(Vector3.zero));
        stop.bunkers.Add(newBunker.GetComponent<Bunker>());
    }

    public static void RemoveBunker(Bunker bunker)
    {
        foreach (Stop stop in Track.Instance.stops)
        {
            if (stop.bunkers.Contains(bunker)) stop.bunkers.Remove(bunker);
        }
        GameObject.DestroyImmediate(bunker.gameObject);
    }
    #endregion
}
