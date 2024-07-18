using UnityEngine;

public static class AnimateSea
{
    public static void UpdateSeaHeight(Mesh mesh, float[,] heightMap, int firstUpperLayerVertexIndex, int upperLayerVerticesPerLine) {
        int verticesLength = mesh.vertices.Length;

        int mapWidthHeight = heightMap.GetLength(0);
        float topLeftX = (mapWidthHeight - 1) / -2f;
        float topLeftZ = (mapWidthHeight - 1) / 2f;

        Vector3[] newVertices = mesh.vertices;

        for (int vertexIndex = firstUpperLayerVertexIndex; vertexIndex < verticesLength - upperLayerVerticesPerLine - 4; vertexIndex += 4) {
            int topNorth = vertexIndex;
            int topEast = vertexIndex + 1;
            int topWest = vertexIndex + 2;
            int topSouth = vertexIndex + 3;

            Vector2 samplePoint = new(newVertices[topNorth].x-topLeftX, (newVertices[topNorth].z)*-1 +topLeftZ);
            float quadHeight = heightMap[(int)samplePoint.x, (int)samplePoint.y] * 20f;

            newVertices[topNorth].y = quadHeight;
            newVertices[topEast].y = quadHeight;
            newVertices[topWest].y = quadHeight;
            newVertices[topSouth].y = quadHeight;
        }

        mesh.vertices = newVertices;
    }
}
