using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SeaGeneration))]
public class SeaGeneratorEditor : Editor {
    public override void OnInspectorGUI() {
        SeaGeneration seaGeneration = (SeaGeneration)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Generate sea")) {
            seaGeneration.GenerateMap();
        }
    }
}
