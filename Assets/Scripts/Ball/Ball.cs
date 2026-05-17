using System;
using NUnit.Framework;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class Ball : MonoBehaviour
{
    
    private Rigidbody2D _rigidBody;
    private CircleCollider2D _circleCollider;
    private SpriteRenderer _spriteRenderer;
    
    [SerializeField] private SOBallSize soBallSize;
    public SOBallSize SoBallSize => soBallSize;

    [Header("Damage")] 
    [SerializeField, Min(1)] private int damage;
    
    [Header("Movement")]
    [SerializeField] private float speed = 1.0f;
    
    private Vector2 _direction;
    private Vector2 _refZeroVelocity = Vector2.zero;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _circleCollider = GetComponent<CircleCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        Assert.IsNotNull(SoBallSize);
    }

    private void Start()
    {
        UpdateBallSize();
        //_direction = Vector2.down;
        
        Debug.Log($"[Ball / {name}] Send notification to {BallsManager.Instance.name} : Ball created.");
        BallsManager.Instance.OnBallInitializedNotification(this);
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
            brick.TryHitBrick(damage);
            return;
        }
        
        if (collidedObject.TryGetComponent(out Platform platform))
        {
            _direction = platform.GetNormalizedDirection(transform.position);
            return;
        }

        if (collidedObject.TryGetComponent(out StaticCollider _))
        {
            _direction = Vector2.Reflect(_direction, normal).normalized;
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject collidedObject = other.gameObject;

        if (collidedObject.TryGetComponent(out DeadZoneTrigger _))
        {
            Debug.Log($"[Ball / {name}] Send notification from {BallsManager.Instance.name} : Ball reached dead zone.");
            BallsManager.Instance.OnBallReachDeadZoneNotification(this);
        }
    }

    /// <summary>
    /// Update ball size.
    /// </summary>
    private void UpdateBallSize()
    {
        _spriteRenderer.sprite = SoBallSize.Sprite;
        _circleCollider.radius = SoBallSize.ColliderRadius;
        speed = SoBallSize.Speed;
        damage = SoBallSize.Damage;
    }

    /// <summary>
    /// Update ball's velocity, based on its direction.
    /// </summary>
    private void MoveBall()
    {
        Vector2 currentVelocity = _rigidBody.linearVelocity;
        Vector2 targetVelocity = _direction * speed;
        _rigidBody.linearVelocity = Vector2.SmoothDamp(currentVelocity, targetVelocity, ref _refZeroVelocity, .0f);
    }

    /// <summary>
    /// Update new ball size.
    /// </summary>
    /// <param name="newSoBallSize"></param>
    public void SetUpNewBallSize(SOBallSize newSoBallSize)
    {
        if (newSoBallSize == null || newSoBallSize.BallSize.Equals(SoBallSize.BallSize))
        {
            return;
        }
        
        soBallSize = newSoBallSize;
        UpdateBallSize();
    }
    
}
