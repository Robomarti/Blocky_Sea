using UnityEngine;

public static class AnimateSea
{
    public static void UpdateSeaHeight(MeshData meshData, float[,] heightMap) {
        int verticesLength = meshData.vertices.Length;
        int firstUpperLayerVertexIndex = meshData.firstUpperLayerVertexIndex;
        int upperLayerVerticesPerLine = meshData.upperLayerVerticesPerLine;

        //meshData.vertices[firstUpperLayerVertexIndex].y = 20;
        //meshData.vertices[firstUpperLayerVertexIndex+2].y = 20;

        for (int vertexIndex = firstUpperLayerVertexIndex; vertexIndex < verticesLength; vertexIndex += 4) {
            // Ensure we are not on the lower or right border before moving triangles
            if (vertexIndex < verticesLength - upperLayerVerticesPerLine) {

                int topNorth = vertexIndex + 3;
                int topEast = vertexIndex + 6;
                int topWest = upperLayerVerticesPerLine + vertexIndex + 1;
                int topSouth = upperLayerVerticesPerLine + vertexIndex + 4;

                float quadHeight = Random.Range(5,20);

                meshData.vertices[topNorth].y = quadHeight;
                meshData.vertices[topEast].y = quadHeight;
                meshData.vertices[topSouth].y = quadHeight;
                meshData.vertices[topWest].y = quadHeight;
            }
        }
    }
}
