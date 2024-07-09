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
        int lowerLayerVertexIndex = 0;

        //lower vertices
        for (int y = 0; y < mapWidthHeight; y += meshSimplificationIncrement) {
            for (int x = 0; x < mapWidthHeight; x += meshSimplificationIncrement) {
                meshData.lowerLayerVertices[lowerLayerVertexIndex] = new Vector3(topLeftX + x, 0, topLeftZ - y);
                meshData.lowerLayerUVs[lowerLayerVertexIndex] = new Vector2(x / (float)mapWidthHeight, y / (float)mapWidthHeight);

                if (x < mapWidthHeight - 1 && y < mapWidthHeight - 1) {
                    meshData.AddLowerTriangle(lowerLayerVertexIndex, verticesPerLine + lowerLayerVertexIndex + 1, verticesPerLine + lowerLayerVertexIndex);
                    meshData.AddLowerTriangle(verticesPerLine + lowerLayerVertexIndex + 1, lowerLayerVertexIndex, lowerLayerVertexIndex + 1);
                }

                lowerLayerVertexIndex += 1;
            }
        }

        int upperLayerVertexIndex = 0;
        //upper vertices
        for (int y = 0; y < mapWidthHeight; y += meshSimplificationIncrement) {
            for (int x = 0; x < mapWidthHeight; x += meshSimplificationIncrement) {
                meshData.upperLayerVertices[upperLayerVertexIndex] = new Vector3(topLeftX + x, 10, topLeftZ - y);
                meshData.upperLayerUVs[upperLayerVertexIndex] = new Vector2(x / (float)mapWidthHeight, y / (float)mapWidthHeight);

                if (x < mapWidthHeight - 1 && y < mapWidthHeight - 1) {
                    meshData.AddUpperTriangle(upperLayerVertexIndex, verticesPerLine + upperLayerVertexIndex + 1, verticesPerLine + upperLayerVertexIndex);
                    meshData.AddUpperTriangle(verticesPerLine + upperLayerVertexIndex + 1, upperLayerVertexIndex, upperLayerVertexIndex + 1);
                        // also create additional vertex
                        if (x > 0 && y > 0) {
                            upperLayerVertexIndex += 1;
                            meshData.upperLayerVertices[upperLayerVertexIndex] = new Vector3(topLeftX + x, 10, topLeftZ - y);
                            meshData.upperLayerUVs[upperLayerVertexIndex] = new Vector2(x / (float)mapWidthHeight, y / (float)mapWidthHeight);
                        }
                }

                upperLayerVertexIndex += 1;
            }
        }

        return meshData;
    }
}

public class MeshData {
    public Vector3[] lowerLayerVertices;
    public Vector2[] lowerLayerUVs;
    public int[] lowerLayerTriangles;
    private int lowerLayerTriangleIndex;

    public Vector3[] upperLayerVertices;
    public Vector2[] upperLayerUVs;
    public int[] upperLayerTriangles;
    private int upperLayerTriangleIndex;

    public MeshData(int verticesPerLine) {
        // we generate upper vertices on top of the regular vertices, and upper layer has almost twice the amout of vertices to keep each quad completely independent
        // we dont generate additional vertices for corner and edge parts of the mesh.
        lowerLayerVertices = new Vector3[verticesPerLine * verticesPerLine];
        lowerLayerUVs = new Vector2[verticesPerLine * verticesPerLine];
        lowerLayerTriangles = new int[(verticesPerLine - 1) * (verticesPerLine - 1) * 6];

        upperLayerVertices = new Vector3[verticesPerLine * verticesPerLine + (verticesPerLine-2) * (verticesPerLine-2)];
        upperLayerUVs = new Vector2[verticesPerLine * verticesPerLine + (verticesPerLine-2) * (verticesPerLine-2)];
        upperLayerTriangles = new int[((verticesPerLine - 1) * (verticesPerLine - 1) + (verticesPerLine-2) * (verticesPerLine-2)) * 6];
    }

    public void AddLowerTriangle(int a,int b,int c) {
        lowerLayerTriangles[lowerLayerTriangleIndex] = a;
        lowerLayerTriangles[lowerLayerTriangleIndex + 1] = b;
        lowerLayerTriangles[lowerLayerTriangleIndex + 2] = c;

        lowerLayerTriangleIndex += 3;
    }

    public void AddUpperTriangle(int a,int b,int c) {
        lowerLayerTriangles[upperLayerTriangleIndex] = a;
        lowerLayerTriangles[upperLayerTriangleIndex + 1] = b;
        lowerLayerTriangles[upperLayerTriangleIndex + 2] = c;

        upperLayerTriangleIndex += 3;
    }

    public Mesh CreateMesh() {
        Mesh mesh = new Mesh();
        mesh.vertices = lowerLayerVertices.Concat(upperLayerVertices).ToArray();
        mesh.uv = lowerLayerUVs.Concat(upperLayerUVs).ToArray();
        mesh.triangles = lowerLayerTriangles.Concat(upperLayerTriangles).ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }
}
