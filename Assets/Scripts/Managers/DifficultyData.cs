using System;
using UnityEngine;

[Serializable]
public class DifficultyData
{
    [SerializeField] private int _speedIncreaseFrequency = 20;
    [SerializeField] private float _difficultyMultiplier = 0.8f;

    public int SpeedIncreaseFrequency { get { return _speedIncreaseFrequency; } }
    public float DifficultyMultiplier { get { return _difficultyMultiplier; } }
}