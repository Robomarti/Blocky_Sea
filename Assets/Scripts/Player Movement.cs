using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    private Vector2 movementDirection;
    [SerializeField] private InputActionReference movement;

    [SerializeField] private Transform seaChunksParent;
    private float lastFullXCoordinate;
    private float lastFullZCoordinate;

    private bool needToUpdateSeaChunkPosition = false;
    [SerializeField] private WaveManager waveManager;
    private int largestLevelOfDetail = -1;

    private void Start() {
        GameObject seaManager = GameObject.Find("Sea Manager");
        largestLevelOfDetail = seaManager.GetComponent<CreateSeaChunks>().detailLevels[^1].levelOfDetail;
    }

    private void Update() {
        // Player movement
        movementDirection = movement.action.ReadValue<Vector2>();
        transform.position += movementSpeed * Time.deltaTime * new Vector3(movementDirection.x, 0, movementDirection.y);

        // Sea chunks movement.
        // We check if the difference in position is larger than largestLevelOfDetail+1 
        // because cubes of a LOD largestLevelOfDetail chunk are largestLevelOfDetail+1 times larger than
        // cubes of a LOD 0 chunk
        if (Mathf.Abs(Mathf.Floor(transform.position.x) - lastFullXCoordinate) >= largestLevelOfDetail+1) {
            lastFullXCoordinate = Mathf.Floor(transform.position.x);
            needToUpdateSeaChunkPosition = true;
        }
        if (Mathf.Abs(Mathf.Floor(transform.position.z) - lastFullZCoordinate) >= largestLevelOfDetail+1) {
            lastFullZCoordinate = Mathf.Floor(transform.position.z);
            needToUpdateSeaChunkPosition = true;
        }

        if (needToUpdateSeaChunkPosition) {
            seaChunksParent.position = new Vector3(lastFullXCoordinate, seaChunksParent.position.y, lastFullZCoordinate);
            waveManager.SeaOffset = new Vector2(lastFullXCoordinate, lastFullZCoordinate);
        }
    }
}
