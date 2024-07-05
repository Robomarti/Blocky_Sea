using System;
using Unity.VisualScripting;
using UnityEngine;

public class Wind : MonoBehaviour
{
    public Vector2 windDirection;

    private void Start()
    {
        windDirection = windDirection.normalized;
    }
}
