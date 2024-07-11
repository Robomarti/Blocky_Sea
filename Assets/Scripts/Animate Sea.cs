using UnityEngine;

public static class AnimateSea
{
    public static void UpdateSeaHeight(MeshData meshData, float[,] heightMap) { 
        int firstUpperLayerVertexIndex = meshData.firstUpperLayerVertexIndex;
        int upperLayerVerticesPerLine = meshData.upperLayerVerticesPerLine;

        meshData.vertices[firstUpperLayerVertexIndex].y = 20;
        meshData.vertices[firstUpperLayerVertexIndex+2].y = 20;

        int vertexCount = 1;
        for (int y = 0; y < upperLayerVerticesPerLine; y += 2) {
            for (int x = 0; x < upperLayerVerticesPerLine; x += 2) {
                // Ensure we are not on the lower or right border before moving triangles
                if (x < upperLayerVerticesPerLine - 2 && y < upperLayerVerticesPerLine - 2) {
                    int vertexIndex = firstUpperLayerVertexIndex + vertexCount;

                    int topNorth = vertexIndex;
                    int topEast = vertexIndex + 1;
                    int topSouth = upperLayerVerticesPerLine + vertexIndex + 1;
                    int topWest = upperLayerVerticesPerLine + vertexIndex;

                    float quadHeight = Random.Range(5,20);

                    meshData.vertices[topNorth].y = quadHeight;
                    meshData.vertices[topEast].y = quadHeight;
                    meshData.vertices[topSouth].y = quadHeight;
                    meshData.vertices[topWest].y = quadHeight;
                }
                vertexCount += 2;
            }  
        }
    }
}
