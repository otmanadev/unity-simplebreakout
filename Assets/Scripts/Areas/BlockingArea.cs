using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

public class BlockingArea : Area, IBallCollisionHandler
{
    
    private readonly List<int> _triggeredBalls = new();
    
    [Header("Ball Properties")]
    [SerializeField] private GameObject ballCollisionAudioPrefab;

    protected override void Awake()
    {
        base.Awake();
        
        Assert.IsNotNull(ballCollisionAudioPrefab);
        Assert.IsTrue(ballCollisionAudioPrefab.GetComponent<Audio>());
        
        CompositeCollider2D.enabled = true;
        TilemapCollider2D.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        GameObject collidedObject = other.gameObject;

        if (!collidedObject.TryGetComponent(out Ball ball))
            return;
        
        ball.RegisterCollision(this, other);
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
        CompositeCollider2D.isTrigger = true;
        areaUI.SetActive(false);
        UpdateAreaColor();
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
        
        CompositeCollider2D.isTrigger = false;
        areaUI.SetActive(true);
        UpdateAreaColor();
    }

    public CollisionResponse HandleBallCollision(Collision2D collision, Vector2 ballDirection)
    {
        Instantiate(ballCollisionAudioPrefab, collision.transform.position, Quaternion.identity);
        Vector2 normal = collision.GetContact(0).normal;
        Vector2 reflectedDirection = Vector2.Reflect(ballDirection, normal);
        return new CollisionResponse(reflectedDirection);
    }
}
