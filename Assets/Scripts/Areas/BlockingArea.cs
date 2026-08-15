using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BlockingArea : Area
{
    
    protected List<int> _triggeredBalls = new List<int>();

    protected override void Awake()
    {
        base.Awake();
        _compositeCollider2D.enabled = true;
        _tilemapCollider2D.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        GameObject collidedObject = other.gameObject;

        if (collidedObject.TryGetComponent(out Ball ball))
        {
            
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject collidedObject = other.gameObject;

        if (collidedObject.TryGetComponent(out Ball ball) && !_triggeredBalls.Contains(ball.GetInstanceID()))
        {
            _triggeredBalls.Add(ball.GetInstanceID());
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        GameObject collidedObject = other.gameObject;
        
        if (collidedObject.TryGetComponent(out Ball ball) && _triggeredBalls.Contains(ball.GetInstanceID()))
        {
            _triggeredBalls.Remove(ball.GetInstanceID());
            CheckIfAreaCanBeEnabled();
        }
    }

    public override void EnableArea()
    {
        isActive = true;
        CheckIfAreaCanBeEnabled();
    }

    public override void DisableArea()
    {
        isActive = false;
        _compositeCollider2D.isTrigger = true;
        areaUI.SetActive(false);
    }

    /// <summary>
    /// Vérifie que l'on peut activer l'Area si les conditions sont respectées :
    /// - Est censé être actif
    /// - N'a aucune balle dans la zone
    /// </summary>
    private void CheckIfAreaCanBeEnabled()
    {
        if (!isActive)
            return;

        if (_triggeredBalls.Count > 0)
            return;
        
        _compositeCollider2D.isTrigger = false;
        areaUI.SetActive(true);
    }
    
}
