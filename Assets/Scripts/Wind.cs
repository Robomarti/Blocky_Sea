using System.Collections;
using UnityEngine;

public class Wind: MonoBehaviour
{
    [SerializeField] private Vector2 windDirection;
    [SerializeField] private float windStrength;
    [SerializeField] private float waveFrequency;
    [SerializeField] private RectTransform windDirectionImageTransform;
    [SerializeField] private Material seaMaterial;

    private void OnValidate() {
        windDirection.x = Mathf.Clamp(windDirection.x, -1f, 1f);
        windDirection.y = Mathf.Clamp(windDirection.y, -1f, 1f);

        // ensure that it is always just a little bit windy
        if (windDirection.x == 0) {
            windDirection.x = 0.1f;
        }
        if (windDirection.y == 0) {
            windDirection.y = 0.1f;
        }

        UpdateWindInfluence();
    }

    private void UpdateWindInfluence() {
        seaMaterial.SetVector("_Wave_Direction", windDirection);
        seaMaterial.SetFloat("_Wave_Strength", windStrength);
        seaMaterial.SetFloat("_Wave_Frequency", waveFrequency);
    }
}
