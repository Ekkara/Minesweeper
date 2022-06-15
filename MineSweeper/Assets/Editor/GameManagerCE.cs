using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class ObjectBuilderEditor : Editor
{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        MapGenerator instance = (MapGenerator)target;

        if (GUILayout.Button("GenerateMap")) {
            instance.GenerateTiles();
        }
        if (GUILayout.Button("RemoveMap")) {
            instance.RemoveTiles();
        }
    }
}