using UnityEngine;

public class DisplaySea : MonoBehaviour {
    public MeshFilter meshFilter;

    public void DrawMesh(Mesh mesh) {
        meshFilter.sharedMesh = mesh;
    }
}
