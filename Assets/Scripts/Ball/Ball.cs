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
    
    [Header("Ball Properties")]
    [SerializeField] private BallSizeSO ballSizeSo;
    public BallSizeSO BallSizeSo => ballSizeSo;
    
    [Header("Movement")]
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float smoothTimeSpeed = .05f;
    private Vector2 _direction;
    private Vector2 _refZeroVelocity = Vector2.zero;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _circleCollider = GetComponent<CircleCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        Assert.IsNotNull(BallSizeSo);
    }

    private void Start()
    {
        UpdateBallSize();
        _direction = Vector2.down;
        
        Debug.Log($"[Ball / {name}] Send notification to {BallsManager.Instance.name} : Ball created.");
        BallsManager.Instance.OnBallCreatedNotification(this);
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
        _spriteRenderer.sprite = BallSizeSo.Sprite;
        _circleCollider.radius = BallSizeSo.ColliderRadius;
        speed = BallSizeSo.Speed;
        smoothTimeSpeed = BallSizeSo.SmoothTime;
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

    /// <summary>
    /// Update new ball size.
    /// </summary>
    /// <param name="newBallSizeSo"></param>
    public void SetUpNewBallSize(BallSizeSO newBallSizeSo)
    {
        if (newBallSizeSo == null || newBallSizeSo.BallSizeType.Equals(BallSizeSo.BallSizeType))
        {
            return;
        }
        
        ballSizeSo = newBallSizeSo;
        UpdateBallSize();
    }
    
}
