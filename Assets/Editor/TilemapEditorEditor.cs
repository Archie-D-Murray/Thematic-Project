using UnityEditor;
using UnityEngine;
using LevelEditor;

[CustomEditor(typeof(TilemapEditor))]
public class TilemapEditorEditor : Editor {
    public override void OnInspectorGUI() {
        TilemapEditor editor = target as TilemapEditor;
        if (GUILayout.Button("Populate UI")) {
            editor.PopulateUI();
        }
        DrawDefaultInspector();
    }
}