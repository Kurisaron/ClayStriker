using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Track))]
[CanEditMultipleObjects()]
public class TrackEditor : Editor
{
    private SerializedProperty showDebugProp;
    private SerializedProperty stopsProp;


    private void OnEnable()
    {
        // Setup Serialized Properties
        showDebugProp = serializedObject.FindProperty("showDebug");
        stopsProp = serializedObject.FindProperty("stops");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //DrawDefaultInspector();

        Track track = (Track)target;

        if (GUILayout.Button("Add Stop"))
        {
            TrackUtils.AddStop(track);
        }

        if (track != null && track.stops != null) EditorGUILayout.LabelField(track.stops.Count.ToString() + " Stops");

        //EditorGUILayout.PropertyField(stopsProp);

        showDebugProp.boolValue = EditorGUILayout.Toggle(new GUIContent("Show Stop Debug", "Show the gizmos in the scene to allow for easier testing"), showDebugProp.boolValue);

        serializedObject.ApplyModifiedProperties();
    }

}
