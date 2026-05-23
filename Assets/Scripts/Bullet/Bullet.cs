using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{

    private Rigidbody2D _rigidBody;
    
    [Min(.0f)] public float speed;
    [Min(0)] public int damage;
    
    private bool _hasReflection = false;
    private Collider2D _collidedGameObject = null;
    private Vector2 _refZeroVelocity;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        HandleDetection();
        MoveBullet();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject collidedObject = other.gameObject;

        if (collidedObject.TryGetComponent(out Brick _)
            || collidedObject.TryGetComponent(out StaticCollider _))
        {
            _hasReflection = true;
            _collidedGameObject = other;
        }
    }

    private void MoveBullet()
    {
        Vector2 currentVelocity = _rigidBody.linearVelocity;
        Vector2 targetVelocity = Vector2.up * speed;
        _rigidBody.linearVelocity = Vector2.SmoothDamp(currentVelocity, targetVelocity, ref _refZeroVelocity, .0f);
    }

    private void HandleDetection()
    {
        if (!_hasReflection)
            return;
        
        GameObject collidedObject = _collidedGameObject.gameObject;
        
        Debug.Log($"{name} collided with {collidedObject.name} on position {transform.position}");
        
        if (collidedObject.TryGetComponent(out Brick brick))
        {
            brick.TryHitBrick(damage);
        }

        _hasReflection = false;
        _collidedGameObject = null;
        Destroy(gameObject);
    }
    
}
