using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class Platform : MonoBehaviour
{
    
    private static readonly String AnimationTriggerSpawn = "SpawnTrigger";
    private static readonly String AnimationTriggerPlatformBigger = "BiggerPlatformTrigger";
    private static readonly String AnimationTriggerPlatformSmaller = "SmallerPlatformTrigger";
    
    private Rigidbody2D _rigidBody;
    private BoxCollider2D _boxCollider;
    private Animator _animator;
    
    [SerializeField] private EPlatformSize platformSize;
    public EPlatformSize PlatformSize => platformSize;

    [Header("Movement")]
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float smoothTimeSpeed = .05f;
    
    private float _inputHorizontalDirection = .0f;
    public float InputHorizontalDirection { set => _inputHorizontalDirection = value; }
    
    private float _refZeroVelocity = .0f;
    
    private float _yPosition;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _animator = GetComponent<Animator>();
        
        _yPosition = transform.position.y;
    }
    
    private void Start()
    {
        UpdatePlatformSize(PlatformSize);
        
        Debug.Log($"[Platform / {name}] Send notification to Platforms Manager : <color=orange>Platform initialized</color>");
        PlatformsManager.Instance.OnPlatformInitializedNotification(this);
    }

    private void FixedUpdate()
    {
        MovePlatform();
        FixVerticalVelocity();
    }
    
    /// <summary>
    /// Play platform spawn animation.
    /// </summary>
    public void SpawnPlatform()
    {
        Debug.Log($"[Platform / {name}] Start animation");
        _animator.SetTrigger(AnimationTriggerSpawn);
    }

    /// <summary>
    /// Notify Platforms Manager the platform finished his spawn animation.
    /// </summary>
    public void OnReceiveNotificationFromAnimatorPlatformSpawned()
    {
        Debug.Log($"[Platform / {name}] Send notification to Platforms Manager : <color=orange>Platform spawned</color>");
        PlatformsManager.Instance.OnPlatformSpawnedNotification(this);
    }
    
    /// <summary>
    /// Update platform size.
    /// </summary>
    /// <param name="newPlatformSize"></param>
    private void UpdatePlatformSize(EPlatformSize newPlatformSize)
    {
        platformSize = newPlatformSize;
        SOPlatformSize platformSizeProperties = PlatformsManager.Instance.GetPlatformSizeByItsType(platformSize);

        _boxCollider.size = platformSizeProperties.ColliderSize;
        speed = platformSizeProperties.Speed;
        smoothTimeSpeed = platformSizeProperties.SmoothTime;
    }

    /// <summary>
    /// Returns normalized direction from given position.
    /// For example : the ball collides with the platform and needs to be sent back.
    /// </summary>
    /// <param name="givenPosition"></param>
    /// <returns></returns>
    public Vector2 GetNormalizedDirection(Vector2 givenPosition)
    {
        Vector2 currentPosition = new Vector2(transform.position.x, transform.position.y);
        return (givenPosition - currentPosition).normalized;
    }

    /// <summary>
    /// Update platform's velocity, based on player inputs.
    /// </summary>
    private void MovePlatform()
    {
        float currentHorizontalVelocity = _rigidBody.linearVelocityX;
        float targetHorizontalVelocity = _inputHorizontalDirection * speed;
        _rigidBody.linearVelocityX = Mathf.SmoothDamp(currentHorizontalVelocity, targetHorizontalVelocity, ref _refZeroVelocity, smoothTimeSpeed);
    }

    private void FixVerticalVelocity()
    {
        _rigidBody.linearVelocityY = .0f;
        transform.position = new Vector2(transform.position.x, _yPosition);
    }
    
    /// <summary>
    /// Update new platform size.
    /// </summary>
    /// <param name="powerUpType"></param>
    /// <param name="newPlatformSizeType"></param>
    public void SetUpNewPlatformSize(EPowerUp powerUpType, EPlatformSize newPlatformSizeType)
    {
        if (PlatformSize.Equals(newPlatformSizeType))
            return;
        
        switch (powerUpType)
        {
            case EPowerUp.PlatformBigger:
                _animator.SetTrigger(AnimationTriggerPlatformBigger);
                break;
            case EPowerUp.PlatformSmaller:
                _animator.SetTrigger(AnimationTriggerPlatformSmaller);
                break;
        }
        
        UpdatePlatformSize(newPlatformSizeType);
    }
    
}
