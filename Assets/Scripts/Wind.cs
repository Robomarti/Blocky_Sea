using System;
using UnityEngine;
using UnityEngine.UI;

public class Wind: MonoBehaviour
{
    [SerializeField] private Vector2 windDirection;
    [SerializeField] private float windStrength;
    [SerializeField] private Image windDirectionImage;

    public Vector2 GetWind()
    {
        return windDirection.normalized * windStrength;
    }
}
