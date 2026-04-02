using System;
using NUnit.Framework;
using UnityEngine;

public class Ball : MonoBehaviour
{
    
    private Rigidbody2D _rigidBody;
    
    [Header("Movement")]
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float smoothTimeSpeed = .05f;
    private Vector2 _direction;
    private Vector2 _refZeroVelocity = Vector2.zero;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(_rigidBody);
    }

    private void Start()
    {
        _direction = Vector2.down;
    }

    private void FixedUpdate()
    {
        MoveBall();
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        GameObject collidedObject = other.gameObject;
        ContactPoint2D contact = other.GetContact(0);
        Vector2 normal = contact.normal;
        
        Debug.Log($"{name} collided with {collidedObject.name} on position {normal}");
        
        if (collidedObject.TryGetComponent(out Brick brick))
        {
            _direction = Vector2.Reflect(_direction, normal).normalized;
            brick.TryHitBrick();
            return;
        }
        
        if (collidedObject.TryGetComponent(out Platform platform))
        {
            _direction = platform.GetNormalizedDirection(transform.position);
            return;
        }
        
        _direction = Vector2.Reflect(_direction, normal).normalized;
        
    }

    /// <summary>
    /// Update ball's velocity, based on its direction.
    /// </summary>
    private void MoveBall()
    {
        Vector2 currentVelocity = _rigidBody.linearVelocity;
        Vector2 targetVelocity = _direction * speed;
        _rigidBody.linearVelocity = Vector2.SmoothDamp(currentVelocity, targetVelocity, ref _refZeroVelocity, smoothTimeSpeed);
    }
    
}
