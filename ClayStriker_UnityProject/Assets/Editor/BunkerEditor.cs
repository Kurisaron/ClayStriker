using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Bunker))]
[CanEditMultipleObjects()]
public class BunkerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Bunker bunker = (Bunker)target;

        if (GUILayout.Button("Remove Bunker"))
        {
            TrackUtils.RemoveBunker(bunker);
        }

        serializedObject.ApplyModifiedProperties();

        DrawDefaultInspector();
    }
}
