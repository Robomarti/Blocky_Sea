using UnityEngine;

public class Wind: MonoBehaviour
{
    [SerializeField] private Vector2 windDirection;
    [SerializeField] private float windStrength;
    [SerializeField] private RectTransform windDirectionImageTransform;
    public Vector2 seaOffset = new Vector2(0,0);

        private void OnValidate() {
        if (windDirection.x < -1) {
            windDirection.x = -1;
        }
        if (windDirection.x > 1) {
            windDirection.x = 1;
        }
        if (windDirection.y < -1) {
            windDirection.y = -1;
        }
        if (windDirection.y > 1) {
            windDirection.y = 1;
        }
    }

    private void Update() {
        seaOffset += windStrength * Time.deltaTime * windDirection.normalized;
        //windDirectionImageTransform.rotation = Quaternion.Euler(0, 0, Vector2.SignedAngle(Vector2.zero, windDirection));
    }
}
