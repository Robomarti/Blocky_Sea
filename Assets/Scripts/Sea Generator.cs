using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;

public class SeaGenerator : MonoBehaviour {
    [SerializeField] private enum DrawMode { Mesh, HeightMap };
    [SerializeField] private DrawMode drawMode;
    [SerializeField] private Noise.NormalizeMode normalizeMode;
    public enum ChunkSize {
        _48 = 49,
        _72 = 73,
        _96 = 97,
    }
    public ChunkSize seaChunkSize;
    [Range(0,5)] public int EditorPreviewLevelOfDetail;
    [SerializeField] private int seed;
    [SerializeField] private float noiseScale;
    [SerializeField] private int octaves;
    [Range(0,1)] [SerializeField] private float persistence;
    [SerializeField] private float lacunarity;
    [SerializeField] private float meshHeightMultiplier;
    public bool autoUpdate;

    private Queue<SeaThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<SeaThreadInfo<MeshData>>();

    public void DrawSeaInEditor() {
        DisplaySea displaySea = FindAnyObjectByType<DisplaySea>();
        if (drawMode == DrawMode.Mesh) {
            MeshData seaMesh = SeaMeshGenerator.GenerateSeaMesh((int)seaChunkSize, EditorPreviewLevelOfDetail);
            Mesh mesh = seaMesh.CreateMesh();
            displaySea.DrawMesh(mesh);
        }
    }

    public void RequestMeshData(Action<MeshData> callback, int levelOfDetail) {
        ThreadStart threadStart = delegate {
            MeshDataThread(callback, levelOfDetail);
        };

        new Thread(threadStart).Start();
    }

    private void MeshDataThread(Action<MeshData> callback, int levelOfDetail) {
        MeshData meshData = SeaMeshGenerator.GenerateSeaMesh((int)seaChunkSize, levelOfDetail);
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

    private void OnValidate() {
        if (lacunarity < 1) {
            lacunarity = 1;
        }
        if (octaves < 0) {
            octaves = 0;
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
