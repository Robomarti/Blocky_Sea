using System;
using UnityEngine;

public static class SeaMeshGenerator {
    public static MeshData GenerateSeaMesh(int mapWidthHeight, int levelOfDetail, int triangleGenerationMode, bool renderLowerTriangles, float topVerticesHeight) {
        float topLeftX = (mapWidthHeight - 1) / -2f;
        float topLeftZ = (mapWidthHeight - 1) / 2f;

        // LOD can't be 5 due to most ChunkSize values not being divisible by 10
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

                if (renderLowerTriangles) {
                    int lowNorth = vertexIndex;
                    int lowEast = vertexIndex + 1;
                    int lowWest = verticesPerLine + vertexIndex;
                    int lowSouth = verticesPerLine + vertexIndex + 1;

                    if (x < mapWidthHeight - 1 && y < mapWidthHeight - 1) {
                        meshData.AddTriangle(lowNorth, lowSouth, lowWest);
                        meshData.AddTriangle(lowSouth, lowNorth, lowEast);
                    }
                }

                vertexIndex += 1;
            }
        }

        int firstUpperLayerVertexIndex = vertexIndex;
        meshData.firstUpperLayerVertexIndex = firstUpperLayerVertexIndex;

        //we can think of upper layer rows either as twice as long with 1 extra row between rows, or simply as 4 times as long as lower layer rows
        int upperLayerVerticesPerLine = verticesPerLine * 4;
        meshData.upperLayerVerticesPerLine = upperLayerVerticesPerLine;

        // Upper layer vertices
        for (int y = 0; y < mapWidthHeight; y += meshSimplificationIncrement) {
            for (int x = 0; x < mapWidthHeight; x += meshSimplificationIncrement) {
                if (x < mapWidthHeight - 1 && y < mapWidthHeight - 1) {
                    int topNorth = vertexIndex;
                    int topEast = vertexIndex + 1;
                    int topWest =  vertexIndex + 2;
                    int topSouth =  vertexIndex + 3;

                    // save same data for each vertex of a quad, so shader can animate them at the same height
                    float quadCenterX = ((topLeftX + x) + (topLeftX + x + meshSimplificationIncrement)) / 2;
                    float quadCenterY = ((topLeftZ - y) + (topLeftZ - y - meshSimplificationIncrement)) / 2;

                    meshData.vertices[topNorth] = new Vector3(topLeftX + x, topVerticesHeight, topLeftZ - y);
                    meshData.uvs[topNorth] = new Vector2(x / (float)mapWidthHeight, y / (float)mapWidthHeight);
                    meshData.quadIdentifiers[topNorth] = new Vector2(quadCenterX, quadCenterY);

                    meshData.vertices[topEast] = new Vector3(topLeftX + x + meshSimplificationIncrement, topVerticesHeight, topLeftZ - y);
                    meshData.uvs[topEast] = new Vector2((x + meshSimplificationIncrement) / (float)mapWidthHeight, y / (float)mapWidthHeight);
                    meshData.quadIdentifiers[topEast] = new Vector2(quadCenterX, quadCenterY);

                    meshData.vertices[topWest] = new Vector3(topLeftX + x, topVerticesHeight, topLeftZ - y - meshSimplificationIncrement);
                    meshData.uvs[topWest] = new Vector2(x / (float)mapWidthHeight, (y - meshSimplificationIncrement) / (float)mapWidthHeight);
                    meshData.quadIdentifiers[topWest] = new Vector2(quadCenterX, quadCenterY);

                    meshData.vertices[topSouth] = new Vector3(topLeftX + x + meshSimplificationIncrement, topVerticesHeight, topLeftZ - y - meshSimplificationIncrement);
                    meshData.uvs[topSouth] = new Vector2((x + meshSimplificationIncrement) / (float)mapWidthHeight, (y - meshSimplificationIncrement) / (float)mapWidthHeight);
                    meshData.quadIdentifiers[topSouth] = new Vector2(quadCenterX, quadCenterY);

                    
                    int lowNorthVertexIndex = (int)MathF.Floor((vertexIndex - firstUpperLayerVertexIndex) / 4);

                    int lowNorth = lowNorthVertexIndex;
                    int lowEast = lowNorthVertexIndex + 1;
                    int lowWest = verticesPerLine + lowNorthVertexIndex;
                    int lowSouth = verticesPerLine + lowNorthVertexIndex + 1;

                    // Top triangles, up facing
                    meshData.AddTriangle(topNorth, topSouth, topWest);
                    meshData.AddTriangle(topSouth, topNorth, topEast);

                    // Northeast triangles, back facing
                    if (triangleGenerationMode == 3) {
                        meshData.AddTriangle(topEast, lowNorth, lowEast);
                        meshData.AddTriangle(lowNorth, topEast, topNorth);
                    }

                    // Northwest triangles, left facing
                    if (triangleGenerationMode != 1) {
                        meshData.AddTriangle(topNorth, lowWest, lowNorth);
                        meshData.AddTriangle(lowWest, topNorth, topWest);
                    }

                    // Southeast triangles, right facing
                    if (triangleGenerationMode != 1) {
                        meshData.AddTriangle(topSouth, lowEast, lowSouth);
                        meshData.AddTriangle(lowEast, topSouth, topEast);
                    }

                    // Southwest triangles, front facing
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
    public Vector2[] quadIdentifiers;
    public int[] triangles;
    private int triangleIndex;

    public int firstUpperLayerVertexIndex;
    public int upperLayerVerticesPerLine;

    public MeshData(int verticesPerLine) {
        int spaceToAllocateToLowerLayer = verticesPerLine * verticesPerLine;
        // We generate upper vertices on top of the regular vertices, and upper layer has 4 times the amount of vertices to keep each quad completely independent
        // Upper layer has twice the amount of vertices horizontally and twice the amount of vertices vertically
        int spaceToAllocateToUpperLayer = 2 * verticesPerLine * 2 * verticesPerLine;
        int spaceToAllocate = spaceToAllocateToLowerLayer + spaceToAllocateToUpperLayer;
        vertices = new Vector3[spaceToAllocate];
        uvs = new Vector2[spaceToAllocate];
        quadIdentifiers = new Vector2[spaceToAllocate];
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
        mesh.SetUVs(0, uvs);
        mesh.SetUVs(1, quadIdentifiers);
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }
}
