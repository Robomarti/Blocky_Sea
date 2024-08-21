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

    private void Update() {
        movementDirection = movement.action.ReadValue<Vector2>();
        transform.position += movementSpeed * Time.deltaTime * new Vector3(movementDirection.x, 0, movementDirection.y);

        // Move sea chunks
        // We check if the difference in position is larger than 6 because 5 is the
        // largest level of detail, and each LOD 5 chunk's cube is 6 times larger than
        // a LOD 0 chunk's cube
        if (Mathf.Abs(Mathf.Floor(transform.position.x) - lastFullXCoordinate) >= 6) {
            lastFullXCoordinate = Mathf.Floor(transform.position.x);
            needToUpdateSeaChunkPosition = true;
        }
        if (Mathf.Abs(Mathf.Floor(transform.position.z) - lastFullZCoordinate) >= 6) {
            lastFullZCoordinate = Mathf.Floor(transform.position.z);
            needToUpdateSeaChunkPosition = true;
        }

        if (needToUpdateSeaChunkPosition) {
            seaChunksParent.position = new Vector3(lastFullXCoordinate, seaChunksParent.position.y, lastFullZCoordinate);
            waveManager.SeaOffset = new Vector2(lastFullXCoordinate, lastFullZCoordinate);
        }
    }
}
