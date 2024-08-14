using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    private Vector2 movementDirection;
    [SerializeField] private InputActionReference movement;

    private void Update() {
        movementDirection = movement.action.ReadValue<Vector2>();
    }

    private void FixedUpdate() {
        transform.position += movementSpeed * Time.deltaTime * new Vector3(movementDirection.x, 0, movementDirection.y) ;
    }
}
