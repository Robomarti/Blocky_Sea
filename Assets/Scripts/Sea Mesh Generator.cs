using System.Linq;
using UnityEngine;

public static class SeaMeshGenerator {
    public static MeshData GenerateSeaMesh(int mapWidthHeight, int levelOfDetail) {
        float topLeftX = (mapWidthHeight - 1) / -2f;
        float topLeftZ = (mapWidthHeight - 1) / 2f;

        //lod can't be 5 due to most ChunkSize values not being divisible by 10
        if (levelOfDetail == 5) {
            levelOfDetail = 6;
        }
        int meshSimplificationIncrement = (levelOfDetail == 0) ? 1 : levelOfDetail * 2;
        int verticesPerLine = (mapWidthHeight - 1) / meshSimplificationIncrement + 1;

        MeshData meshData = new MeshData(verticesPerLine);
        int vertexIndex = 0;

        //lower vertices
        for (int y = 0; y < mapWidthHeight; y += meshSimplificationIncrement) {
            for (int x = 0; x < mapWidthHeight; x += meshSimplificationIncrement) {
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, 0, topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)mapWidthHeight, y / (float)mapWidthHeight);

                if (x < mapWidthHeight - 1 && y < mapWidthHeight - 1) {
                    meshData.AddTriangle(vertexIndex, verticesPerLine + vertexIndex + 1, verticesPerLine + vertexIndex);
                    meshData.AddTriangle(verticesPerLine + vertexIndex + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex += 1;
            }
        }

        // Upper vertices
        for (int y = 0; y < mapWidthHeight; y += meshSimplificationIncrement) {
            for (int x = 0; x < mapWidthHeight; x += meshSimplificationIncrement) {
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, 10, topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)mapWidthHeight, y / (float)mapWidthHeight);

                // Ensure we are not on the border before adding triangles
                if (x < mapWidthHeight - 1 && y < mapWidthHeight - 1) {
                    // Adjust vertex index for center parts
                    int adjustedVerticesPerLine = verticesPerLine;
                    // Create additional vertex in the center parts
                    if (x > 0 && y > 0) {
                        //adjustedVerticesPerLine = verticesPerLine + (verticesPerLine - 2);
                        vertexIndex += 1;
                        meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, 10, topLeftZ - y);
                        meshData.uvs[vertexIndex] = new Vector2(x / (float)mapWidthHeight, y / (float)mapWidthHeight);
                    }

                    meshData.AddTriangle(vertexIndex, adjustedVerticesPerLine + vertexIndex + 1, adjustedVerticesPerLine + vertexIndex);
                    meshData.AddTriangle(adjustedVerticesPerLine + vertexIndex + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex += 1;
            }
        }
        Debug.Log(vertexIndex);
        Debug.Log(verticesPerLine);
        return meshData;
    }
}

public class MeshData {
    public Vector3[] vertices;
    public Vector2[] uvs;
    public int[] triangles;
    private int triangleIndex;

    public MeshData(int verticesPerLine) {
        // we generate upper vertices on top of the regular vertices, and upper layer has almost twice the amout of vertices to keep each quad completely independent
        // we dont generate additional vertices for corner and edge parts of the mesh.
        int spaceToAllocate = verticesPerLine * verticesPerLine + (verticesPerLine * verticesPerLine + (verticesPerLine-2) * (verticesPerLine-2));
        vertices = new Vector3[spaceToAllocate];
        uvs = new Vector2[spaceToAllocate];
        int trianglesToAllocate = (verticesPerLine - 1) * (verticesPerLine - 1) * 6 + (((verticesPerLine - 1) * (verticesPerLine - 1) + (verticesPerLine-2) * (verticesPerLine-2)) * 6);
        triangles = new int[trianglesToAllocate];

        //upperLayerVertices = new Vector3[verticesPerLine * verticesPerLine + (verticesPerLine-2) * (verticesPerLine-2)];
        //upperLayerUVs = new Vector2[verticesPerLine * verticesPerLine + (verticesPerLine-2) * (verticesPerLine-2)];
        //upperLayerTriangles = new int[((verticesPerLine - 1) * (verticesPerLine - 1) + (verticesPerLine-2) * (verticesPerLine-2)) * 6];
    }

    public void AddTriangle(int a,int b,int c) {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;

        triangleIndex += 3;
    }

    public Mesh CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }
}
