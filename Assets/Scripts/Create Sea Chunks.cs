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

    static SeaGenerator seaGenerator;

    private void Start() {
        seaGenerator = FindAnyObjectByType<SeaGenerator>();
        maxViewDistance = detailLevels[detailLevels.Length-1].visibleDistanceThreshold;
        chunkSize = SeaGenerator.seaChunkSize-1;
        chunksVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / chunkSize);
        viewerPosition = new Vector2(viewer.position.x,viewer.position.z);
        CreateVisibleChunks();
    }

    private void CreateVisibleChunks() {
        int currentChunkXCoordinate = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkYCoordinate = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for (int yOffset = -chunksVisibleInViewDistance; yOffset <= chunksVisibleInViewDistance; yOffset++) {
            for (int xOffset = -chunksVisibleInViewDistance; xOffset <= chunksVisibleInViewDistance; xOffset++) {
                Vector2 viewedChunkCoordinates = new Vector2(currentChunkXCoordinate + xOffset, currentChunkYCoordinate + yOffset);
                new SeaChunk(viewedChunkCoordinates, chunkSize, transform, seaMaterial, detailLevels);
            }
        }
    }

    public class SeaChunk {
        private GameObject meshObject;
        private Vector2 position;
        private Bounds bounds;
        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;
        private int levelOfDetail = -1;

        public SeaChunk(Vector2 coordinates, int size, Transform parent, Material material, LevelOfDetailInfo[] detailLevels) {
            position = coordinates * size;
            bounds = new Bounds(position,Vector2.one * size);
            Vector3 chunkPosition = new Vector3(position.x, 0, position.y);

            float viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));

            for (int i = 0; i < detailLevels.Length-1; i++){
                if (viewerDistanceFromNearestEdge <= detailLevels[i].visibleDistanceThreshold) {
                    levelOfDetail = detailLevels[i].levelOfDetail;
                    break;
                }
            }

            if (levelOfDetail == -1) {
                levelOfDetail = detailLevels[detailLevels.Length-1].levelOfDetail;
            }

            meshObject = new GameObject("Sea chunk "+levelOfDetail.ToString()+" "+viewerDistanceFromNearestEdge.ToString()+" ");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshRenderer.material = material;
            meshObject.transform.position = chunkPosition;
            meshObject.transform.parent = parent;

            seaGenerator.RequestSeaData(OnSeaDataReceived);
        }

        private void OnSeaDataReceived(SeaData seaData) {
            LevelOfDetailMesh levelOfDetailMesh = new LevelOfDetailMesh(levelOfDetail, meshFilter);
            levelOfDetailMesh.RequestMesh(seaData);
        }
    }

    class LevelOfDetailMesh {
        public Mesh mesh;
        private int levelOfDetail;
        private MeshFilter meshFilter;

        public LevelOfDetailMesh(int receivedLevelOfDetail, MeshFilter receivedMeshFilter) {
            levelOfDetail = receivedLevelOfDetail;
            meshFilter = receivedMeshFilter;
        }

        private void OnMeshDataReceived(MeshData meshData) {
            mesh = meshData.CreateMesh();
            meshFilter.mesh = mesh;
        }

        public void RequestMesh(SeaData seaData) {
            seaGenerator.RequestMeshData(seaData, OnMeshDataReceived, levelOfDetail);
        }
    }

    [System.Serializable] public struct LevelOfDetailInfo {
        public int levelOfDetail;
        public float visibleDistanceThreshold;
    }
}
