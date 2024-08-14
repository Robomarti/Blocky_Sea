using UnityEngine;
using UnityEngine.EventSystems;

public class DragWaveDirection : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    // combination of these tutorials:
    // samyam's https://youtu.be/XCoDKZqa-PE?si=Z29ZyTkkQH84I33x
    // devsplorer's https://youtu.be/V4AJWjvOFBw?si=3lP4sk3yWdiN-kHC

    private RectTransform draggableObject;
    private float screenWidth;
    private Vector2 dragStartPosition;
    private Quaternion dragStartRotation;

    [SerializeField] private WaveManager waveManager;

    private void Awake() {
        draggableObject = transform as RectTransform;
    }
    
    private void Start() {
        screenWidth = Screen.width;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragStartPosition = eventData.position;
        dragStartRotation = transform.rotation;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(draggableObject, eventData.position, eventData.pressEventCamera, out var globalMousePosition)) {
            
            float currentDistanceBetweenMousePositions = globalMousePosition.x - dragStartPosition.x;
            transform.rotation = dragStartRotation * Quaternion.Euler(-360 * (currentDistanceBetweenMousePositions / screenWidth) * Vector3.forward);
            float zAngle = transform.rotation.eulerAngles.z;
            Vector2 rotationToVector2 = new Vector2(Mathf.Sin(zAngle * Mathf.Deg2Rad), Mathf.Cos(zAngle * Mathf.Deg2Rad));
            rotationToVector2.y *= -1f;
            waveManager.WaveDirection = rotationToVector2;
        }
    }
}
