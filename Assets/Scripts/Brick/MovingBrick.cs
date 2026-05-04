using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class MovingBrick : Brick
{

    [Header("Brick Movement")]
    [SerializeField, Min(0f)] private float moveSpeed;
    [SerializeField] private List<Transform> coordinates;

    protected override void Awake()
    {
        base.Awake();
        Assert.IsNotNull(coordinates);
        Assert.IsTrue(coordinates.Count > 0);
    }
    
}
