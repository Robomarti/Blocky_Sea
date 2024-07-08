using UnityEngine;

public static class Noise {
    public static float[,] GenerateNoiseMap(int mapWidthHeight, int seed, float scale, int octaves, float persistence, float lacunarity, Vector2 offset) {
        float[,] noiseMap = new float[mapWidthHeight,mapWidthHeight];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        for (int i = 0; i < octaves; i++) {
            float offsetX = prng.Next(-100000,100000) + offset.x;
            float offsetY = prng.Next(-100000,100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0) {
            scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfMapWidthHeight = mapWidthHeight / 2f;

        for (int y = 0; y < mapWidthHeight; y++) {
            for (int x = 0; x < mapWidthHeight; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++) {
                    float sampleX = ((x - halfMapWidthHeight) / scale - octaveOffsets[i].x) * frequency * -1f;
                    float sampleY = ((y - halfMapWidthHeight) / scale - octaveOffsets[i].y) * frequency * -1f;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight) {
                    maxNoiseHeight = noiseHeight;
                }
                if (noiseHeight < minNoiseHeight) {
                    minNoiseHeight = noiseHeight;
                }

                noiseMap[x,y] = noiseHeight;
            }
        }

        for (int y = 0; y < mapWidthHeight; y++) {
            for (int x = 0; x < mapWidthHeight; x++) {
                noiseMap[x,y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight,noiseMap[x,y]);
            }
        }

        return noiseMap;
    }
}
