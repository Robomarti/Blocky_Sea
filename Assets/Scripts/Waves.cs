

using System.Collections.Generic;
using UnityEngine;

public class Waves : MonoBehaviour
{
    private Vector2 waveDirection;
    private float waveHeight;
    [SerializeField] private Wind windManager;

    private void Start()
    {
        if (windManager != null)
        {
            waveDirection = windManager.windDirection;
            waveHeight = windManager.windStrength;
        }
    }
}
