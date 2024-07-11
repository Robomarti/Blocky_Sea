using System;
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

        // Lower layer vertices
        for (int y = 0; y < mapWidthHeight; y += meshSimplificationIncrement) {
            for (int x = 0; x < mapWidthHeight; x += meshSimplificationIncrement) {
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, 0, topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)mapWidthHeight, y / (float)mapWidthHeight);

                int lowNorth = vertexIndex;
                int lowEast = vertexIndex + 1;
                int lowWest = verticesPerLine + vertexIndex;
                int lowSouth = verticesPerLine + vertexIndex + 1;

                if (x < mapWidthHeight - 1 && y < mapWidthHeight - 1) {
                    meshData.AddTriangle(lowNorth, lowSouth, lowWest);
                    meshData.AddTriangle(lowSouth, lowNorth, lowEast);
                }

                vertexIndex += 1;
            }
        }

        int firstUpperLayerVertexIndex = vertexIndex;
        meshData.firstUpperLayerVertexIndex = firstUpperLayerVertexIndex;

        // Upper layer vertices
        for (int y = 0; y < mapWidthHeight; y += meshSimplificationIncrement) {
            for (int x = 0; x < mapWidthHeight; x += meshSimplificationIncrement) {
                // Add 4 vertices for upper layer for each vertex of lower layer
                for (int i = 0; i < 4; i++) {
                    meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, 10, topLeftZ - y);
                    meshData.uvs[vertexIndex] = new Vector2(x / (float)mapWidthHeight, y / (float)mapWidthHeight);
                    vertexIndex += 1;
                }
            }
        }

        vertexIndex = firstUpperLayerVertexIndex;

        //we can think of upper layer lines either as 2 as long with 1 extra line between lines, or simply as 4 times as long as lower layer lines
        int upperLayerVerticesPerLine = verticesPerLine * 4;
        meshData.upperLayerVerticesPerLine = upperLayerVerticesPerLine;

        // cube faces with upper vertices
        for (int y = 0; y < mapWidthHeight; y += meshSimplificationIncrement) {
            for (int x = 0; x < mapWidthHeight; x += meshSimplificationIncrement) {
                if (x < mapWidthHeight - 1 && y < mapWidthHeight - 1) {
                    int topNorth = vertexIndex + 3;
                    int topEast = vertexIndex + 6;
                    int topWest = upperLayerVerticesPerLine + vertexIndex + 1;
                    int topSouth = upperLayerVerticesPerLine + vertexIndex + 4;
                    
                    int correspondingLowerLayerVertexIndex = (int)MathF.Floor((vertexIndex - firstUpperLayerVertexIndex) / 4);

                    int lowNorth = correspondingLowerLayerVertexIndex;
                    int lowEast = correspondingLowerLayerVertexIndex + 1;
                    int lowSouth = verticesPerLine + correspondingLowerLayerVertexIndex + 1;
                    int lowWest = verticesPerLine + correspondingLowerLayerVertexIndex;

                    // Top triangles
                    meshData.AddTriangle(topNorth, topSouth, topWest);
                    meshData.AddTriangle(topSouth, topNorth, topEast);

                    // Northeast triangles
                    meshData.AddTriangle(topEast, lowNorth, lowEast);
                    meshData.AddTriangle(lowNorth, topEast, topNorth);

                    // Northwest triangles
                    meshData.AddTriangle(topNorth, lowWest, lowNorth);
                    meshData.AddTriangle(lowWest, topNorth, topWest);

                    // Southeast triangles
                    meshData.AddTriangle(topSouth, lowEast, lowSouth);
                    meshData.AddTriangle(lowEast, topSouth, topEast);

                    // Southwest triangles
                    meshData.AddTriangle(topWest, lowSouth, lowWest);
                    meshData.AddTriangle(lowSouth, topWest, topSouth);
                }
                vertexIndex += 4;
            }
        }

        return meshData;
    }
}

public class MeshData {
    public Vector3[] vertices;
    public Vector2[] uvs;
    public int[] triangles;
    private int triangleIndex;

    public int firstUpperLayerVertexIndex;
    public int upperLayerVerticesPerLine;

    public MeshData(int verticesPerLine) {
        int spaceToAllocateToLowerLayer = verticesPerLine * verticesPerLine;
        // We generate upper vertices on top of the regular vertices, and upper layer has 4 times the amount of vertices to keep each quad completely independent
        // We generate useless additional vertices for corner and edge parts of the mesh because coding hard :(.
        // Upper layer has twice the amount of vertices horizontally and twice the amount of vertices vertically
        int spaceToAllocateToUpperLayer = 2 * verticesPerLine * 2 * verticesPerLine;
        int spaceToAllocate = spaceToAllocateToLowerLayer + spaceToAllocateToUpperLayer;
        vertices = new Vector3[spaceToAllocate];
        uvs = new Vector2[spaceToAllocate];
        // 6 vertices make up 2 triangles which make 1 quad. 6 quads make a cube
        int trianglesToAllocate = (verticesPerLine - 1) * (verticesPerLine - 1) * 6 * 6;
        triangles = new int[trianglesToAllocate];
    }

    public void AddTriangle(int a, int b, int c) {
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
