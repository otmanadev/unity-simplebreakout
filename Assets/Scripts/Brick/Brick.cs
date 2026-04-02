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
        Assert.IsNotNull(_spriteRenderer);
        
        _boxCollider2D = GetComponent<BoxCollider2D>();
        Assert.IsNotNull(_boxCollider2D);
    }

    /// <summary>
    /// Hit brick, so he could die... Or not...
    /// </summary>
    public void TryHitBrick()
    {
        Destroy(gameObject);
    }
    
}
