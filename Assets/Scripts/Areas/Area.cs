using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteAlways]
[RequireComponent(typeof(CompositeCollider2D))]
[RequireComponent(typeof(TilemapCollider2D))]
public abstract class Area : MonoBehaviour
{
    
    protected CompositeCollider2D CompositeCollider2D;
    protected TilemapCollider2D TilemapCollider2D;
    
    [Header("References")]
    [SerializeField] protected GameObject areaUI;

    [Header("Area Properties")] 
    [SerializeField] protected bool isActive;
    [SerializeField] private Color colorWhenActive;
    [SerializeField] private Color colorWhenInactive;
    public Color AreaColor => isActive ? colorWhenActive : colorWhenInactive;

    protected virtual void Awake()
    {
        CompositeCollider2D = GetComponent<CompositeCollider2D>();
        TilemapCollider2D = GetComponent<TilemapCollider2D>();
        
        Assert.IsNotNull(areaUI);
        
        UpdateAreaColor();
    }

    protected void UpdateAreaColor()
    {
        if (CompositeCollider2D.gameObject.TryGetComponent(out Tilemap tilemapComposite))
        {
            tilemapComposite.color = AreaColor;
        }
        
        if (areaUI.TryGetComponent(out Tilemap tilemapUI))
        {
            tilemapUI.color = AreaColor;
        }
    }

    public virtual void EnableArea()
    {
        isActive = true;
        CompositeCollider2D.enabled = true;
        TilemapCollider2D.enabled = true;
        areaUI.SetActive(true);
        UpdateAreaColor();
    }

    public virtual void DisableArea()
    {
        isActive = false;
        CompositeCollider2D.enabled = false;
        TilemapCollider2D.enabled = false;
        areaUI.SetActive(false);
        UpdateAreaColor();
    }

    private void OnValidate()
    {
        CompositeCollider2D = GetComponent<CompositeCollider2D>();
        TilemapCollider2D = GetComponent<TilemapCollider2D>();
        if (areaUI != null)
            UpdateAreaColor();
    }
}
