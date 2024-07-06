using UnityEngine;

public class SeaGeneration : MonoBehaviour {
    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;
    [SerializeField] private float noiseScale;

    public void GenerateMap() {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight,noiseScale);

        DisplaySea displaySea = FindObjectOfType<DisplaySea>();
        displaySea.DrawNoiseMap(noiseMap);
    }
}
