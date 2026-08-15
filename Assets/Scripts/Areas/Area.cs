using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(CompositeCollider2D))]
[RequireComponent(typeof(TilemapCollider2D))]
public abstract class Area : MonoBehaviour
{
    
    protected CompositeCollider2D _compositeCollider2D;
    protected TilemapCollider2D _tilemapCollider2D;
    
    [Header("References")]
    [SerializeField] protected GameObject areaUI;

    [Header("Properties")] 
    [SerializeField] protected bool isActive;
    public Color color;

    protected virtual void Awake()
    {
        _compositeCollider2D = GetComponent<CompositeCollider2D>();
        _tilemapCollider2D = GetComponent<TilemapCollider2D>();
        
        Assert.IsNotNull(areaUI);
        
        UpdateAreaColor(color);
    }

    private void UpdateAreaColor(Color newColor)
    {
        color = newColor;

        if (_compositeCollider2D.gameObject.TryGetComponent(out Tilemap tilemapComposite))
        {
            tilemapComposite.color = newColor;
        }
        
        if (areaUI.TryGetComponent(out Tilemap tilemapUI))
        {
            tilemapUI.color = newColor;
        }
    }

    public virtual void EnableArea()
    {
        isActive = true;
        _compositeCollider2D.enabled = true;
        _tilemapCollider2D.enabled = true;
        areaUI.SetActive(true);
    }

    public virtual void DisableArea()
    {
        isActive = false;
        _compositeCollider2D.enabled = false;
        _tilemapCollider2D.enabled = false;
        areaUI.SetActive(false);
    }

    private void OnValidate()
    {
        _compositeCollider2D = GetComponent<CompositeCollider2D>();
        _tilemapCollider2D = GetComponent<TilemapCollider2D>();
        if (areaUI != null)
            UpdateAreaColor(color);
    }
}
