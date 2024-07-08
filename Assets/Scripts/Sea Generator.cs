using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;

public class SeaGenerator : MonoBehaviour {
    [SerializeField] private enum DrawMode { Mesh };
    [SerializeField] private DrawMode drawMode;

    public const int seaChunkSize = 241;
    [Range(0,6)] public int EditorPreviewLevelOfDetail;
    [SerializeField] private int seed;
    [SerializeField] private float noiseScale;
    [SerializeField] private int octaves;
    [Range(0,1)] [SerializeField] private float persistence;
    [SerializeField] private float lacunarity;
    [SerializeField] private Vector2 offset;

    [SerializeField] private float meshHeightMultiplier;

    public bool autoUpdate;

    private Queue<SeaThreadInfo<SeaData>> seaDataThreadInfoQueue = new Queue<SeaThreadInfo<SeaData>>();
    private Queue<SeaThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<SeaThreadInfo<MeshData>>();

    public void DrawSeaInEditor() {
        SeaData seaData = GenerateSeaData();

        SeaDisplay seaDisplay = FindAnyObjectByType<SeaDisplay>();
        if (drawMode == DrawMode.Mesh) {
            seaDisplay.DrawMesh(SeaMeshGenerator.GenerateSeaMesh(seaData.heightMap, meshHeightMultiplier, EditorPreviewLevelOfDetail));
        } else {
            seaDisplay.DrawNoiseMap(seaData.heightMap);
        }
    }

    public void RequestSeaData(Action<SeaData> callback) {
        ThreadStart threadStart = delegate {
            SeaDataThread(callback);
        };

        new Thread(threadStart).Start();
    }

    private void SeaDataThread(Action<SeaData> callback) {
        SeaData seaData = GenerateSeaData();
        lock(seaDataThreadInfoQueue) {
            seaDataThreadInfoQueue.Enqueue(new SeaThreadInfo<SeaData>(callback, seaData));
        }
    }

    public void RequestMeshData(SeaData seaData, Action<MeshData> callback, int levelOfDetail) {
        ThreadStart threadStart = delegate {
            MeshDataThread(seaData, callback, levelOfDetail);
        };

        new Thread(threadStart).Start();
    }

    private void MeshDataThread(SeaData seaData, Action<MeshData> callback, int levelOfDetail) {
        MeshData meshData = SeaMeshGenerator.GenerateSeaMesh(seaData.heightMap, meshHeightMultiplier, levelOfDetail);
        lock (meshDataThreadInfoQueue) {
            meshDataThreadInfoQueue.Enqueue(new SeaThreadInfo<MeshData>(callback, meshData));
        }
    }

    private void Update(){
        if (seaDataThreadInfoQueue.Count > 0) {
            for (int i = 0; i < seaDataThreadInfoQueue.Count; i++) {
                SeaThreadInfo<SeaData> threadInfo = seaDataThreadInfoQueue.Dequeue();
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

    private SeaData GenerateSeaData() {
        float[,] noiseMap = Noise.GenerateNoiseMap(seaChunkSize, seed, noiseScale, octaves, persistence, lacunarity, offset);
        return new SeaData(noiseMap);
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

public readonly struct SeaData {
    public readonly float[,] heightMap;

    public SeaData(float[,] heightMap) {
        this.heightMap = heightMap;
    }
}