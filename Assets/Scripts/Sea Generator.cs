using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;

public class SeaGenerator : MonoBehaviour {
    private enum DrawMode { Mesh, None };
    [SerializeField] private DrawMode drawMode;
    public enum ChunkSize {
        _48 = 49,
        _72 = 73,
        _96 = 97
    }
    public ChunkSize seaChunkSize;

    public enum TriangleGenerationMode {
        Front_And_Up = 1,
        Front_Left_Right_Up = 2,
        All = 3
    }
    [Tooltip(@"Front_And_Up - The fastest option, but the mesh can only be viewed from the front. 
Front_Left_Right_Up - Generates all the faces for the cube except the back ones. The Mesh can be rotated when the player rotates to hide this.
All - Generates all the faces normally, but the additional triangles slow down rendering.")]
    [SerializeField] private TriangleGenerationMode triangleGenerationMode;

    [Tooltip("Whether to generate triangles of the bottoms of the cube. They are likely not needed in most use cases.")]
    [SerializeField] private bool generateLowerTriangles;

    [Range(0,5)] public int EditorPreviewLevelOfDetail;
    [SerializeField] private float topVerticesHeight;
    public bool autoUpdate;

    [SerializeField] private Material seaMaterial;
    [SerializeField] private Vector3 seaChunkLocation;
    [SerializeField] private float meshSize;

    private Queue<SeaThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<SeaThreadInfo<MeshData>>();

    public void DrawSeaInEditor() {
        DisplaySea displaySea = FindAnyObjectByType<DisplaySea>();
        if (drawMode == DrawMode.Mesh) {
            MeshData seaMesh = SeaMeshGenerator.GenerateSeaMesh((int)seaChunkSize, EditorPreviewLevelOfDetail, (int)triangleGenerationMode, generateLowerTriangles, topVerticesHeight);
            Mesh mesh = seaMesh.CreateMesh();
            displaySea.DrawMesh(mesh);
        }
    }

    public void CreateSeaGameObject() {
        GameObject meshObject = new GameObject("Sea chunk, level of detail: " + EditorPreviewLevelOfDetail.ToString());
        MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
        meshRenderer.material = seaMaterial;
        meshObject.transform.position = seaChunkLocation;
        meshObject.transform.parent = transform;
        meshObject.transform.localScale = Vector3.one * meshSize;

        MeshData seaMesh = SeaMeshGenerator.GenerateSeaMesh((int)seaChunkSize, EditorPreviewLevelOfDetail, (int)triangleGenerationMode, generateLowerTriangles, topVerticesHeight);
        meshFilter.mesh = seaMesh.CreateMesh();
    }

    public void RequestMeshData(Action<MeshData> callback, int levelOfDetail) {
        ThreadStart threadStart = delegate {
            MeshDataThread(callback, levelOfDetail);
        };

        new Thread(threadStart).Start();
    }

    private void MeshDataThread(Action<MeshData> callback, int levelOfDetail) {
        MeshData meshData = SeaMeshGenerator.GenerateSeaMesh((int)seaChunkSize, levelOfDetail, (int)triangleGenerationMode, generateLowerTriangles, topVerticesHeight);
        lock (meshDataThreadInfoQueue) {
            meshDataThreadInfoQueue.Enqueue(new SeaThreadInfo<MeshData>(callback, meshData));
        }
    }

    private void Update(){
        if (meshDataThreadInfoQueue.Count > 0) {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++) {
                SeaThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    private struct SeaThreadInfo<T> {
        public Action<T> callback;
        public T parameter;

        public SeaThreadInfo(Action<T> callback, T parameter) {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}
