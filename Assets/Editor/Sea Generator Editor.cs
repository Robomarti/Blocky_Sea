using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SeaGenerator))]
public class SeaGeneratorEditor : Editor {
    public override void OnInspectorGUI() {
        SeaGenerator seaGeneration = (SeaGenerator)target;

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
