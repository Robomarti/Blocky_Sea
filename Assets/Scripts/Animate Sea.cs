using UnityEngine;

public static class AnimateSea
{
    public static void UpdateSeaHeight(MeshData meshData, float[,] heightMap) {
        int verticesLength = meshData.vertices.Length;
        int firstUpperLayerVertexIndex = meshData.firstUpperLayerVertexIndex;
        int upperLayerVerticesPerLine = meshData.upperLayerVerticesPerLine;

        int mapWidthHeight = heightMap.GetLength(0);
        float topLeftX = (mapWidthHeight - 1) / -2f;
        float topLeftZ = (mapWidthHeight - 1) / 2f;

        for (int vertexIndex = firstUpperLayerVertexIndex; vertexIndex < verticesLength; vertexIndex += 4) {
            // Ensure we are not on the lower or right border before moving triangles
            if (vertexIndex < verticesLength - upperLayerVerticesPerLine - 4) {

                int topNorth = vertexIndex + 3;
                int topEast = vertexIndex + 6;
                int topWest = upperLayerVerticesPerLine + vertexIndex + 1;
                int topSouth = upperLayerVerticesPerLine + vertexIndex + 4;

                Vector2 samplePoint = new(meshData.vertices[topNorth].x-topLeftX, (meshData.vertices[topNorth].z)*-1 +topLeftZ);
                float quadHeight = heightMap[(int)samplePoint.x, (int)samplePoint.y] * 20f;
                //float quadHeight = Random.Range(1,20);

                meshData.vertices[topNorth].y = quadHeight;
                meshData.vertices[topEast].y = quadHeight;
                meshData.vertices[topWest].y = quadHeight;
                meshData.vertices[topSouth].y = quadHeight;
            }
        }
    }
}
