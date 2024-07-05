

using System.Collections.Generic;
using UnityEngine;

public class Waves : MonoBehaviour
{
    private Vector2 waveDirection;
    private float waveHeight;
    [SerializeField] private Wind windManager;
    private List<Transform> armamentBones = new List<Transform>();

    private void Start()
    {
        if (windManager != null)
        {
            waveDirection = windManager.windDirection;
            waveHeight = windManager.windStrength;
        }
            
        Transform armature = transform.GetChild(0);
        foreach (Transform child in armature)
        {
            armamentBones.Add(child);
        }
    }

    private void Update()
    {
        foreach (Transform bone in armamentBones)
        {

        }
    }
}
