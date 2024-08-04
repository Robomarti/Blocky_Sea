using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private float smoothingIterations;
    [SerializeField] private Vector2 waveDirection;
    [SerializeField] private float waveSpeed;
    [SerializeField] private float waveAmplitude;
    [SerializeField] private float wavePersistence;
    [SerializeField] private float wavePeriod;
    [SerializeField] private float waveLacunarity;
    [SerializeField] private RectTransform windDirectionImageTransform;
    [SerializeField] private Material seaMaterial;

    private void OnValidate() {
        waveDirection.x = Mathf.Clamp(waveDirection.x, -1f, 1f);
        waveDirection.y = Mathf.Clamp(waveDirection.y, -1f, 1f);

        UpdateWindInfluence();
    }

    private void UpdateWindInfluence() {
        seaMaterial.SetFloat("_Iterations", smoothingIterations);
        seaMaterial.SetFloat("_Wave_Speed", waveSpeed);
        seaMaterial.SetFloat("_Wave_X_Direction", waveDirection.x);
        seaMaterial.SetFloat("_Wave_Y_Direction", waveDirection.y);
        seaMaterial.SetFloat("_Wave_Amplitude", waveAmplitude);
        seaMaterial.SetFloat("_Persistence", wavePersistence);
        seaMaterial.SetFloat("_Wave_Period", wavePeriod);
        seaMaterial.SetFloat("_Lacunarity", waveLacunarity);
    }
}
