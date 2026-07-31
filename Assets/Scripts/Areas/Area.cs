using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(CompositeCollider2D))]
[RequireComponent(typeof(TilemapCollider2D))]
public abstract class Area : MonoBehaviour
{
    
    private CompositeCollider2D _compositeCollider2D;
    private TilemapCollider2D _tilemapCollider2D;
    protected List<Ball> _triggeredBalls;
    
    [Header("References")]
    [SerializeField] protected GameObject areaUI;

    [Header("Properties")] 
    [SerializeField] protected bool isActive;
    [SerializeField] private Color color;

    private void Awake()
    {
        _compositeCollider2D = GetComponent<CompositeCollider2D>();
        _tilemapCollider2D = GetComponent<TilemapCollider2D>();
        _triggeredBalls = new List<Ball>();
        
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

    private void EnableArea()
    {
        isActive = true;
        _compositeCollider2D.enabled = true;
        _tilemapCollider2D.enabled = true;
        areaUI.SetActive(true);
    }

    private void DisableArea()
    {
        isActive = false;
        _compositeCollider2D.enabled = false;
        _tilemapCollider2D.enabled = false;
        areaUI.SetActive(false);
    }
    
}
