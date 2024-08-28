using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SeaGenerator))]
public class SeaGeneratorEditor : Editor {
    public override void OnInspectorGUI() {
        SeaGenerator seaGenerator = (SeaGenerator)target;

        if (DrawDefaultInspector()) {
            if (seaGenerator.autoUpdate) {
                seaGenerator.DrawSeaInEditor();
            }
        }

        if (GUILayout.Button("Update SeaMesh Object")) {
            seaGenerator.DrawSeaInEditor();
        }

        if (GUILayout.Button("Create A New Sea Chunk Object")) {
            seaGenerator.CreateSeaGameObject();
        }
    }
}
