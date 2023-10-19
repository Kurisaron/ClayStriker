using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Stop))]
[CanEditMultipleObjects()]
public class StopEditor : Editor
{
    private SerializedProperty showDebugProp;
    private SerializedProperty trackProp;
    private SerializedProperty arrivalAngleProp;
    private SerializedProperty bunkersProp;
    private SerializedProperty firingSequenceProp;

    private void OnEnable()
    {
        // Setup Serialized Properties
        showDebugProp = serializedObject.FindProperty("showDebug");
        trackProp = serializedObject.FindProperty("track");
        arrivalAngleProp = serializedObject.FindProperty("arrivalAngle");
        bunkersProp = serializedObject.FindProperty("bunkers");
        firingSequenceProp = serializedObject.FindProperty("firingSequence");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        //DrawDefaultInspector();

        Stop stop = (Stop)target;

        if (GUILayout.Button("Remove Stop"))
        {
            TrackUtils.RemoveStop(stop);
        }

        if (TrackUtils.ValidateContextSwitchStop())
        {
            if (GUILayout.Button("Switch Stops"))
            {
                TrackUtils.SwitchStops();
            }
        }

        EditorGUILayout.Space(15);

        EditorGUILayout.PropertyField(bunkersProp);
        if (GUILayout.Button("Add Bunker"))
        {
            TrackUtils.AddBunker(stop);
        }
        EditorGUILayout.PropertyField(firingSequenceProp);

        EditorGUILayout.Space(15);
        arrivalAngleProp.floatValue = EditorGUILayout.Slider(new GUIContent("Arrival Direction Angle", "Change this angle to change which direction the player will be facing upon arriving at the stop"), arrivalAngleProp.floatValue, 0, 360);

        showDebugProp.boolValue = EditorGUILayout.Toggle(new GUIContent("Show Bunker Debug", "Show the gizmos in the scene to allow for easier testing"), showDebugProp.boolValue);

        serializedObject.ApplyModifiedProperties();

    }

}
