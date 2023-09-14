using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TrackUtils
{

    [MenuItem("GameObject/Clay Strikers/Track/Start Track")]
    public static void StartTrack()
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

    
}
