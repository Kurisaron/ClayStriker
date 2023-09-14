using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Track))]
[CanEditMultipleObjects()]
public class TrackEditor : Editor
{
    private SerializedProperty stopsProp;

    private bool stopsGroupOpen = false;

    private void OnEnable()
    {
        // Setup Serialized Properties
        stopsProp = serializedObject.FindProperty("stops");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //DrawDefaultInspector();

        Track track = (Track)target;

        if (GUILayout.Button("Add Stop"))
        {
            GameObject newStop = new GameObject("Stop" + track.stops.Count);
            newStop.transform.SetPositionRotationAndParent(Vector3.zero, Quaternion.Euler(Vector3.zero), track.transform);
            track.stops.Add(newStop.AddComponent<Stop>().Init(track));
        }

        if (track != null && track.stops != null) EditorGUILayout.LabelField(track.stops.Count.ToString() + " Stops");

        //EditorGUILayout.PropertyField(stopsProp);

    }

}
