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
        if (collidedObject.TryGetComponent(out Platform platform))
        {
            _direction = platform.GetNormalizedDirection(transform.position);
            return;
        }
        
        Debug.Log("Contact with something else...");
        ContactPoint2D contact = other.GetContact(0);
        
        Vector2 normal = contact.normal;
        
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
