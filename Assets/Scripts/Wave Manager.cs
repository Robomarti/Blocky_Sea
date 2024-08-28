using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private float smoothingIterations;
    [SerializeField] private Vector2 waveDirection;
    public Vector2 WaveDirection {
        set { waveDirection = value; UpdateWaveDirection(); }
    }
    [SerializeField] private float waveSpeed;
    [SerializeField] private float waveAmplitude;
    [SerializeField] private float wavePersistence;
    [SerializeField] private float wavePeriod;
    [SerializeField] private float waveLacunarity;
    [SerializeField] private Material seaMaterial;
    [SerializeField] private Vector2 seaOffset;
    public Vector2 SeaOffset {
        set { seaOffset = value; UpdateSeaOffset(); }
    }
    [SerializeField] [Range(1,100)] private float randomnessChance;
    [SerializeField] private float randomnessStrength;

    private void OnValidate() {
        waveDirection.x = Mathf.Clamp(waveDirection.x, -1f, 1f);
        waveDirection.y = Mathf.Clamp(waveDirection.y, -1f, 1f);

        UpdateWindInfluence();
    }

    private void UpdateWindInfluence() {
        seaMaterial.SetFloat("_Iterations", smoothingIterations);
        seaMaterial.SetFloat("_Wave_Speed", waveSpeed);
        seaMaterial.SetFloat("_Wave_Amplitude", waveAmplitude);
        seaMaterial.SetFloat("_Persistence", wavePersistence);
        seaMaterial.SetFloat("_Wave_Period", wavePeriod);
        seaMaterial.SetFloat("_Lacunarity", waveLacunarity);
        seaMaterial.SetFloat("_Randomness_Chance", randomnessChance);
        seaMaterial.SetFloat("_Randomness_Strength", randomnessStrength);
    }

    private void UpdateWaveDirection() {
        seaMaterial.SetFloat("_Wave_X_Direction", waveDirection.x);
        seaMaterial.SetFloat("_Wave_Z_Direction", waveDirection.y);
    }

    private void UpdateSeaOffset() {
        seaMaterial.SetVector("_Sea_Offset", seaOffset);
    }

    private void Start() {
        seaMaterial.SetVector("_Sea_Offset", new Vector2(0,-1));
    }
}
