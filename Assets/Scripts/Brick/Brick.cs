using System;
using NUnit.Framework;
using UnityEngine;

public class Brick : MonoBehaviour
{
    
    private SpriteRenderer _spriteRenderer;
    
    private BoxCollider2D _boxCollider2D;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        
        Assert.IsNotNull(_spriteRenderer, $"[Brick / {name}] SpriteRenderer is null");
        Assert.IsNotNull(_boxCollider2D, $"[Brick / {name}] BoxCollider2D is null");
    }

    private void Start()
    {
        Debug.Log($"[Brick / {name}] Start");
    }
}
