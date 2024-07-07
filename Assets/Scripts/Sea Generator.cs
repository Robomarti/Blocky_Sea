using UnityEngine;

public class SeaGenerator : MonoBehaviour {
    [SerializeField] private enum DrawMode { Mesh };
    [SerializeField] private DrawMode drawMode;

    private const int seaChunkSize = 241;
    [Range(0,6)] public int levelOfDetail;
    [SerializeField] private int seed;
    [SerializeField] private float noiseScale;
    [SerializeField] private int octaves;
    [Range(0,1)] [SerializeField] private float persistence;
    [SerializeField] private float lacunarity;
    [SerializeField] private Vector2 offset;

    [SerializeField] private float meshHeightMultiplier;

    public bool autoUpdate;

    public void GenerateMap() {
        float[,] noiseMap = Noise.GenerateNoiseMap(seaChunkSize, seaChunkSize, seed, noiseScale, octaves, persistence, lacunarity, offset);
        SeaDisplay displaySea = FindAnyObjectByType<SeaDisplay>();
        if (drawMode == DrawMode.Mesh) {
            displaySea.DrawMesh(SeaMeshGenerator.GenerateSeaMesh(noiseMap, meshHeightMultiplier, levelOfDetail));
        } else {
            displaySea.DrawNoiseMap(noiseMap);
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
}
