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
        _120 = 121,
        _144 = 145,
        _168 = 169,
    }
    public ChunkSize seaChunkSize;
    [Range(1,5)] public int EditorPreviewLevelOfDetail;
    [SerializeField] private int seed;
    [SerializeField] private float noiseScale;
    [SerializeField] private int octaves;
    [Range(0,1)] [SerializeField] private float persistence;
    [SerializeField] private float lacunarity;
    [SerializeField] private float meshHeightMultiplier;
    public bool autoUpdate;

    private Queue<SeaThreadInfo<float[,]>> heightMapThreadInfoQueue = new Queue<SeaThreadInfo<float[,]>>();
    private Queue<SeaThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<SeaThreadInfo<MeshData>>();

    [SerializeField] private Wind windManager;

    public void DrawSeaInEditor() {
        float[,] heightMap = GenerateHeightMap(Vector2.zero);

        DisplaySea displaySea = FindAnyObjectByType<DisplaySea>();
        if (drawMode == DrawMode.Mesh) {
            MeshData seaMesh = SeaMeshGenerator.GenerateSeaMesh(heightMap.GetLength(0), EditorPreviewLevelOfDetail);
            Mesh mesh = seaMesh.CreateMesh();
            displaySea.DrawMesh(mesh);
        } 
        else {
            displaySea.DrawNoiseMap(heightMap);
        }
    }

    public void RequestHeightMap(Action<float[,]> callback, Vector2 center) {
        ThreadStart threadStart = delegate {
            heightMapThread(callback, center);
        };

        new Thread(threadStart).Start();
    }

    private void heightMapThread(Action<float[,]> callback, Vector2 center) {
        float[,] heightMap = GenerateHeightMap(center);
        lock(heightMapThreadInfoQueue) {
            heightMapThreadInfoQueue.Enqueue(new SeaThreadInfo<float[,]>(callback, heightMap));
        }
    }

    public void RequestMeshData(float[,] heightMap, Action<MeshData> callback, int levelOfDetail) {
        ThreadStart threadStart = delegate {
            MeshDataThread(heightMap, callback, levelOfDetail);
        };

        new Thread(threadStart).Start();
    }

    private void MeshDataThread(float[,] heightMap, Action<MeshData> callback, int levelOfDetail) {
        MeshData meshData = SeaMeshGenerator.GenerateSeaMesh(heightMap.GetLength(0), levelOfDetail);
        lock (meshDataThreadInfoQueue) {
            meshDataThreadInfoQueue.Enqueue(new SeaThreadInfo<MeshData>(callback, meshData));
        }
    }

    private void Update(){
        if (heightMapThreadInfoQueue.Count > 0) {
            for (int i = 0; i < heightMapThreadInfoQueue.Count; i++) {
                SeaThreadInfo<float[,]> threadInfo = heightMapThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
        if (meshDataThreadInfoQueue.Count > 0) {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++) {
                SeaThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    private float[,] GenerateHeightMap(Vector2 center) {
        float[,] noiseMap = Noise.GenerateNoiseMap((int)seaChunkSize, seed, noiseScale, octaves, persistence, lacunarity, center + windManager.seaOffset, normalizeMode);
        return noiseMap;
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
