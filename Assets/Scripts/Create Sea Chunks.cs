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

        public SeaChunk(Vector2 coordinates, int size, Transform parent, Material material, LevelOfDetailInfo[] detailLevels) {
            position = coordinates * size;
            bounds = new Bounds(position,Vector2.one * size);
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

            CreateSeaMesh();
        }

        private void CreateSeaMesh() {
            LevelOfDetailMesh levelOfDetailMesh = new LevelOfDetailMesh(levelOfDetail, meshFilter);
            levelOfDetailMesh.RequestMesh();
        }
    }

    class LevelOfDetailMesh {
        private int levelOfDetail;
        private MeshFilter meshFilter;

        public LevelOfDetailMesh(int receivedLevelOfDetail, MeshFilter receivedMeshFilter) {
            levelOfDetail = receivedLevelOfDetail;
            meshFilter = receivedMeshFilter;
        }

        private void OnMeshDataReceived(MeshData meshData) {
            meshFilter.mesh = meshData.CreateMesh();
        }

        public void RequestMesh() {
            seaGenerator.RequestMeshData(OnMeshDataReceived, levelOfDetail);
        }
    }

    [System.Serializable] public struct LevelOfDetailInfo {
        public int levelOfDetail;
        public float visibleDistanceThreshold;
    }
}
