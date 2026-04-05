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

    private void Start()
    {
        Debug.Log($"[Brick / {name}] Send notification to {BricksManager.Instance.name} : Brick created.");
        BricksManager.Instance.OnBrickCreatedNotification(this);
    }

    /// <summary>
    /// Hit brick, so he could die... Or not...
    /// </summary>
    public void TryHitBrick()
    {
        Debug.Log($"[Brick / {name}] Send notification to {BricksManager.Instance.name} : Brick destroyed.");
        BricksManager.Instance.OnBrickDestroyedNotification(this);
        Destroy(gameObject);
    }
    
}
