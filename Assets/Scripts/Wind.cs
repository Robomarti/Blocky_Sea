using System;
using Unity.VisualScripting;
using UnityEngine;

public class Wind : MonoBehaviour
{
    public Vector2 windDirection;
    public float windStrength;

    private void Start()
    {
        windDirection = windDirection.normalized;
    }
}
