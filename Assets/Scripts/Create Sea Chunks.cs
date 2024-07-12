using UnityEngine;

public class CreateSeaChunks : MonoBehaviour
{
    public LevelOfDetailInfo[] detailLevels;
    [SerializeField] private static float maxViewDistance;
    [SerializeField] private Transform viewer;
    [SerializeField] private Material seaMaterial;
    public static Vector2 viewerPosition;
    private int chunkSize;
    private int chunksVisibleInViewDistance;
    private const float scale = 1f;
    private SeaChunk[] seaChunks;

    static SeaGenerator seaGenerator;

    private void Start() {
        seaGenerator = FindAnyObjectByType<SeaGenerator>();
        maxViewDistance = detailLevels[^1].visibleDistanceThreshold;
        chunkSize = (int)seaGenerator.seaChunkSize-1;
        chunksVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / chunkSize);
        seaChunks = new SeaChunk[(chunksVisibleInViewDistance * 2 + 1)*(chunksVisibleInViewDistance * 2 + 1)];
        viewerPosition = new Vector2(viewer.position.x,viewer.position.z) / scale;
        CreateVisibleChunks();
    }

    /*
    private void Update() {
        foreach (SeaChunk seaChunk in seaChunks) {
            if (seaChunk.isReadyToAnimate.Value) {
                seaChunk.RequestHeightData(seaChunk.Position);
            }
        }
    }
    */

    private void CreateVisibleChunks() {
        int currentChunkXCoordinate = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkYCoordinate = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        int seaChunkCounter = 0;
        for (int yOffset = -chunksVisibleInViewDistance; yOffset <= chunksVisibleInViewDistance; yOffset++) {
            for (int xOffset = -chunksVisibleInViewDistance; xOffset <= chunksVisibleInViewDistance; xOffset++) {
                Vector2 viewedChunkCoordinates = new Vector2(currentChunkXCoordinate + xOffset, currentChunkYCoordinate + yOffset);
                SeaChunk seaChunk = new SeaChunk(viewedChunkCoordinates, chunkSize, transform, seaMaterial, detailLevels);
                seaChunks[seaChunkCounter] = seaChunk;
                seaChunkCounter += 1;
            }
        }
    }

    public class SeaChunk {
        private GameObject meshObject;
        private Vector2 position;
        public Vector2 Position {
            get { return position; }
        }
        private Bounds bounds;
        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;
        private int levelOfDetail = -1;

        private MyRef<int> firstUpperLayerVertexIndex;
        private MyRef<int> upperLayerVerticesPerLine;

        public MyRef<bool> isReadyToAnimate;

        public SeaChunk(Vector2 coordinates, int size, Transform parent, Material material, LevelOfDetailInfo[] detailLevels) {
            position = coordinates * size;
            bounds = new Bounds(position,Vector2.one * size);
            firstUpperLayerVertexIndex = new MyRef<int> { Value = 0 };
            upperLayerVerticesPerLine = new MyRef<int> { Value = 0 };
            isReadyToAnimate = new MyRef<bool> { Value = false };

            float viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));

            for (int i = 0; i < detailLevels.Length-1; i++){
                if (viewerDistanceFromNearestEdge <= detailLevels[i].visibleDistanceThreshold) {
                    levelOfDetail = detailLevels[i].levelOfDetail;
                    break;
                }
            }

            if (levelOfDetail > 5 || levelOfDetail < 1) {
                levelOfDetail = detailLevels[^1].levelOfDetail;
            }

            meshObject = new GameObject("Sea chunk, level of detail: "+levelOfDetail.ToString());
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;
            meshObject.transform.position = new Vector3(position.x, 0, position.y) * scale;
            meshObject.transform.parent = parent;
            meshObject.transform.localScale = Vector3.one * scale;

            seaGenerator.RequestHeightMap(OnSeaDataReceived, position);
        }

        private void OnSeaDataReceived(float[,] heightMap) {
            LevelOfDetailMesh levelOfDetailMesh = new LevelOfDetailMesh(levelOfDetail, meshFilter, firstUpperLayerVertexIndex, upperLayerVerticesPerLine, isReadyToAnimate);
            levelOfDetailMesh.RequestMesh(heightMap);
        }

        public void RequestHeightData(Vector2 position) {
            seaGenerator.RequestHeightMap(OnHeightDataReceived, position);
        }

        private void OnHeightDataReceived(float[,] heightMap) {
            // wait until the values have been initialized
            if (isReadyToAnimate.Value) {
                AnimateSea.UpdateSeaHeight(meshFilter.mesh, heightMap, firstUpperLayerVertexIndex.Value, upperLayerVerticesPerLine.Value);
            }
        }
    }

    class LevelOfDetailMesh {
        private Mesh mesh;
        private int levelOfDetail;
        private MeshFilter meshFilter;
        private MyRef<int> firstUpperLayerVertexIndex;
        private MyRef<int> upperLayerVerticesPerLine;
        private MyRef<bool> isReadyToAnimate;

        public LevelOfDetailMesh(int receivedLevelOfDetail, MeshFilter receivedMeshFilter, MyRef<int> receivedFirstUpperLayerVertexIndex, MyRef<int> receivedUpperLayerVerticesPerLine, MyRef<bool> receivedIsReadyToAnimate) {
            levelOfDetail = receivedLevelOfDetail;
            meshFilter = receivedMeshFilter;
            firstUpperLayerVertexIndex = receivedFirstUpperLayerVertexIndex;
            upperLayerVerticesPerLine = receivedUpperLayerVerticesPerLine;
            isReadyToAnimate = receivedIsReadyToAnimate;
        }

        private void OnMeshDataReceived(MeshData meshData) {
            firstUpperLayerVertexIndex.Value = meshData.firstUpperLayerVertexIndex;
            upperLayerVerticesPerLine.Value = meshData.upperLayerVerticesPerLine;
            isReadyToAnimate.Value = true;
            mesh = meshData.CreateMesh();
            meshFilter.mesh = mesh;
        }

        public void RequestMesh(float[,] heightMap) {
            seaGenerator.RequestMeshData(heightMap, OnMeshDataReceived, levelOfDetail);
        }
    }

    [System.Serializable] public struct LevelOfDetailInfo {
        public int levelOfDetail;
        public float visibleDistanceThreshold;
    }
}


public sealed class MyRef<T> {
    public T Value { get; set; }
}