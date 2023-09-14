using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Stop))]
[CanEditMultipleObjects()]
public class StopEditor : Editor
{
    private SerializedProperty trackProp;

    private void OnEnable()
    {
        // Setup Serialized Properties
        trackProp = serializedObject.FindProperty("track");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        Stop stop = (Stop)target;

        if (GUILayout.Button("Remove Stop"))
        {
            Track track = stop.track;
            track.stops.Remove(stop);

            for (int i = 0; i < track.stops.Count; ++i)
            {
                Stop stop1 = track.stops[i];
                stop1.gameObject.name = "Stop" + i.ToString();
            }

            DestroyImmediate(stop.gameObject);
        }
    }
}
