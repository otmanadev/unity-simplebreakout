using System;
using UnityEngine;

[Serializable]
public struct DialogTextDuration
{
    
    [SerializeField] private string text;
    public string Text => text;
    
    [SerializeField, Min(.0f)] private float duration;
    public float Duration => duration;
    
}