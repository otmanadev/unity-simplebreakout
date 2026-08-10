using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{

    private Rigidbody2D _rigidBody;

    [Header("Movement")] 
    [SerializeField] private Movement movement;
    public Movement Movement => movement;
    private Vector2 _refZeroVelocity;
    
    [Header("Damage")]
    [Min(0)] public int damage;

    private readonly HashSet<Collider2D> _pendingCollisions = new();

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        movement.UpdateDirectionNormalized(Vector2.up);
    }

    private void FixedUpdate()
    {
        MoveBullet();
        HandleCollisions();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject collidedObject = other.gameObject;

        if (collidedObject.TryGetComponent(out Brick _)
            || collidedObject.TryGetComponent(out StaticCollider _))
        {
            _pendingCollisions.Add(other);
        }
    }

    private void MoveBullet()
    {
        Vector2 currentVelocity = _rigidBody.linearVelocity;
        Vector2 targetVelocity = movement.GetMovementDirection;
        _rigidBody.linearVelocity = Vector2.SmoothDamp(currentVelocity, targetVelocity, ref _refZeroVelocity, .0f);
    }

    /// <summary>
    /// Gère et traite l'ensemble des collisions rencontrées.
    /// </summary>
    private void HandleCollisions()
    {
        if (_pendingCollisions.Count == 0)
            return;

        foreach (Collider2D pendingCollision in _pendingCollisions)
        {
            HandleCollisions(pendingCollision);
        }
        
        _pendingCollisions.Clear();
    }

    /// <summary>
    /// Gère et traite une collision rencontrée.
    /// </summary>
    /// <param name="collision"></param>
    private void HandleCollisions(Collider2D collision)
    {
        GameObject collidedObject = collision.gameObject;
        
        Debug.Log($"{name} collided with {collidedObject.name} on position {transform.position}");
        
        if (collidedObject.TryGetComponent(out Brick brick))
        {
            brick.TryHitBrick(damage);
        }
        
        Destroy(gameObject);
    }
    
}
