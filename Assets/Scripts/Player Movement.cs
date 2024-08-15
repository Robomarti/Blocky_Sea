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

    private void Update() {
        movementDirection = movement.action.ReadValue<Vector2>();
        transform.position += movementSpeed * Time.deltaTime * new Vector3(movementDirection.x, 0, movementDirection.y);
        if (Mathf.Floor(transform.position.x) != lastFullXCoordinate) {
            lastFullXCoordinate = Mathf.Floor(transform.position.x);
            needToUpdateSeaChunkPosition = true;
        }
        if (Mathf.Floor(transform.position.z) != lastFullZCoordinate) {
            lastFullZCoordinate = Mathf.Floor(transform.position.z);
            needToUpdateSeaChunkPosition = true;
        }

        if (needToUpdateSeaChunkPosition) {
            seaChunksParent.position = new Vector3(lastFullXCoordinate, seaChunksParent.position.y, lastFullZCoordinate);
        }
    }
}
