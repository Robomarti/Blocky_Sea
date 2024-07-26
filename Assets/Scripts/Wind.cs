using UnityEngine;

public class Wind: MonoBehaviour
{
    [SerializeField] private Vector2 windDirection;
    [SerializeField] private float windStrength;
    [SerializeField] private RectTransform windDirectionImageTransform;

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

        // ensure that it is always just a little bit windy
        if (windDirection.x == 0) {
            windDirection.x = 0.1f;
        }
        if (windDirection.y == 0) {
            windDirection.y = 0.1f;
        }
    }
}
