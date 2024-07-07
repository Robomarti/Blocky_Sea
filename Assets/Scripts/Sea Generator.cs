using UnityEngine;

public class SeaGenerator : MonoBehaviour {
    [SerializeField] private enum DrawMode { Mesh };
    [SerializeField] private DrawMode drawMode;

    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;
    [SerializeField] private int seed;
    [SerializeField] private float noiseScale;
    [SerializeField] private int octaves;
    [Range(0,1)] [SerializeField] private float persistence;
    [SerializeField] private float lacunarity;
    [SerializeField] private Vector2 offset;

    public bool autoUpdate;

    public void GenerateMap() {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth,mapHeight,seed,noiseScale,octaves,persistence,lacunarity,offset);
        SeaDisplay displaySea = FindAnyObjectByType<SeaDisplay>();
        if (drawMode == DrawMode.Mesh) {
            displaySea.DrawMesh(SeaMeshGenerator.GenerateSeaMesh(noiseMap));
        } else {
            displaySea.DrawNoiseMap(noiseMap);
        }
    }

    private void OnValidate() {
        if (mapWidth < 1) {
            mapWidth = 1;
        }
        if (mapHeight < 1) {
            mapHeight = 1;
        }
        if (lacunarity < 1) {
            lacunarity = 1;
        }
        if (octaves < 0) {
            octaves = 0;
        }
    }
}
