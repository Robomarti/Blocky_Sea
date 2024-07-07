using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SeaGeneration))]
public class SeaGeneratorEditor : Editor {
    public override void OnInspectorGUI() {
        SeaGeneration seaGeneration = (SeaGeneration)target;

        if (DrawDefaultInspector()) {
            if (seaGeneration.autoUpdate) {
                seaGeneration.GenerateMap();
            }
        }

        if (GUILayout.Button("Generate sea")) {
            seaGeneration.GenerateMap();
        }
    }
}
