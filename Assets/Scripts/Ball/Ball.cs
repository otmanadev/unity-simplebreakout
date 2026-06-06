using System;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Animator))]
public class Ball : MonoBehaviour
{
    
    private static readonly String AnimationTriggerSpawn = "SpawnTrigger";
    private static readonly String AnimationTriggerBallBigger = "BiggerBallTrigger";
    private static readonly String AnimationTriggerBallSmaller = "SmallerBallTrigger";
    
    private Rigidbody2D _rigidBody;
    private CircleCollider2D _circleCollider;
    private Animator _animator;

    [SerializeField] private EBallSize ballSize;
    public EBallSize BallSize => ballSize;

    [Header("Damage")] 
    [SerializeField, Min(1)] private int damage;

    private bool _hasReflection;
    private Collision2D _collidedGameObject;
    
    [Header("Movement")]
    [SerializeField] private float speed = 1.0f;
    
    private Vector2 _direction;
    public Vector2 Direction => _direction;
    private Vector2 _refZeroVelocity = Vector2.zero;
    
    [Header("Hit properties")]
    [SerializeField] private GameObject hitStaticColliderAudioPrefab;
    [SerializeField] private GameObject hitPlatformAudioPrefab;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _circleCollider = GetComponent<CircleCollider2D>();
        _animator = GetComponent<Animator>();
        
        Assert.IsNotNull(hitStaticColliderAudioPrefab);
        Assert.IsTrue(hitStaticColliderAudioPrefab.GetComponent<Audio>());
        Assert.IsNotNull(hitPlatformAudioPrefab);
        Assert.IsTrue(hitPlatformAudioPrefab.GetComponent<Audio>());
    }

    private void Start()
    {
        UpdateBallSize(BallSize);
        
        Debug.Log($"[Ball / {name}] Send notification to {BallsManager.Instance.name} : Ball created.");
        BallsManager.Instance.OnBallInitializedNotification(this);
    }

    private void FixedUpdate()
    {
        HandleReflection();
        MoveBall();
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        GameObject collidedObject = other.gameObject;

        if (collidedObject.TryGetComponent(out Brick _)
            || collidedObject.TryGetComponent(out StaticCollider _)
            || collidedObject.TryGetComponent(out Platform _))
        {
            _hasReflection = true;
            _collidedGameObject = other;
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
    /// Start move ball.
    /// </summary>
    public void StartMoveBall()
    {
        _direction = Vector2.down;
    }
    
    /// <summary>
    /// Play ball spawn animation.
    /// </summary>
    public void StartBallSpawn()
    {
        Debug.Log($"[Ball / {name}] Start animation");
        _animator.SetTrigger(AnimationTriggerSpawn);
    }
    
    /// <summary>
    /// Notify Balls Manager the ball finished his spawn animation.
    /// </summary>
    public void OnReceiveNotificationFromAnimatorBallSpawned()
    {
        Debug.Log($"[Ball / {name}] Send notification to Balls Manager : <color=orange>Ball spawned</color>");
        BallsManager.Instance.OnBallSpawnedNotification(this);
    }

    /// <summary>
    /// Update ball size.
    /// </summary>
    /// <param name="newBallSizeType"></param>
    private void UpdateBallSize(EBallSize newBallSizeType)
    {
        ballSize = newBallSizeType;
        SOBallSize ballSizeProperties = BallsManager.Instance.GetBallSizeByItsType(BallSize);
        
        _circleCollider.radius = ballSizeProperties.ColliderRadius;
        speed = ballSizeProperties.Speed;
        damage = ballSizeProperties.Damage;
    }
 
    /// <summary>
    /// Update ball's velocity, based on its direction.
    /// </summary>
    private void MoveBall()
    {
        Vector2 currentVelocity = _rigidBody.linearVelocity;
        float currentSpeed = BallsManager.Instance.MovementMultiplier * speed;
        Vector2 targetVelocity = _direction * currentSpeed;
        _rigidBody.linearVelocity = Vector2.SmoothDamp(currentVelocity, targetVelocity, ref _refZeroVelocity, .0f);
    }

    private void HandleReflection()
    {
        if (!_hasReflection)
            return;
        
        GameObject collidedObject = _collidedGameObject.gameObject;
        ContactPoint2D contact = _collidedGameObject.GetContact(0);
        Vector2 normal = contact.normal;
        
        Debug.Log($"{name} collided with {collidedObject.name} on position {normal}");
        
        if (collidedObject.TryGetComponent(out Brick brick))
        {
            _direction = Vector2.Reflect(_direction, normal).normalized;
            brick.TryHitBrick(damage);
        }
        
        if (collidedObject.TryGetComponent(out Platform platform))
        {
            _direction = platform.GetBallNormalizedDirectionFromGivenPosition(transform.position.x);
            Instantiate(hitPlatformAudioPrefab, transform.position, Quaternion.identity);
        }

        if (collidedObject.TryGetComponent(out StaticCollider _))
        {
            _direction = Vector2.Reflect(_direction, normal).normalized;
            Instantiate(hitStaticColliderAudioPrefab, transform.position, Quaternion.identity);
        }

        _hasReflection = false;
        _collidedGameObject = null;
    }

    /// <summary>
    /// Update new ball size.
    /// </summary>
    /// <param name="powerUpType"></param>
    /// <param name="newBallSizeType"></param>
    public void SetUpNewBallSize(EPowerUp powerUpType, EBallSize newBallSizeType)
    {
        if (BallSize.Equals(newBallSizeType))
            return;

        switch (powerUpType)
        {
            case EPowerUp.BallBigger:
                _animator.SetTrigger(AnimationTriggerBallBigger);
                break;
            case EPowerUp.BallSmaller:
                _animator.SetTrigger(AnimationTriggerBallSmaller);
                break;
        }
        
        UpdateBallSize(newBallSizeType);
    }
    
}
