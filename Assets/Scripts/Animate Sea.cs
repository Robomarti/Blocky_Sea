using UnityEngine;

public static class AnimateSea
{
    public static void UpdateSeaHeight(Mesh mesh, float[,] heightMap, int firstUpperLayerVertexIndex, int upperLayerVerticesPerLine) {
        int verticesLength = mesh.vertices.Length;

        int mapWidthHeight = heightMap.GetLength(0);
        float topLeftX = (mapWidthHeight - 1) / -2f;
        float topLeftZ = (mapWidthHeight - 1) / 2f;

        Vector3[] newVertices = mesh.vertices;

        for (int vertexIndex = firstUpperLayerVertexIndex; vertexIndex < verticesLength; vertexIndex += 4) {
            // Ensure we are not on the lower border before moving triangles
            if (vertexIndex < verticesLength - upperLayerVerticesPerLine - 4) {

                int topNorth = vertexIndex + 3;
                int topEast = vertexIndex + 6;
                int topWest = upperLayerVerticesPerLine + vertexIndex + 1;
                int topSouth = upperLayerVerticesPerLine + vertexIndex + 4;

                Vector2 samplePoint = new(newVertices[topNorth].x-topLeftX, (newVertices[topNorth].z)*-1 +topLeftZ);
                float quadHeight = heightMap[(int)samplePoint.x, (int)samplePoint.y] * 20f;

                newVertices[topNorth].y = quadHeight;
                newVertices[topEast].y = quadHeight;
                newVertices[topWest].y = quadHeight;
                newVertices[topSouth].y = quadHeight;
            }
        }

        mesh.vertices = newVertices;
    }
}
