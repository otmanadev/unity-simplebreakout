using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Audio;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class Platform : MonoBehaviour, IBallCollisionHandler
{
    
    private static readonly String AnimationTriggerSpawn = "SpawnTrigger";
    private static readonly String AnimationTriggerPlatformBigger = "PlatformBiggerTrigger";
    private static readonly String AnimationTriggerPlatformSmaller = "PlatformSmallerTrigger";
    private static readonly String AnimationIntegerPlatformSizeLevel = "PlatformSizeLevel";
    
    private Rigidbody2D _rigidBody;
    private BoxCollider2D _boxCollider;
    private Animator _animator;
    
    [SerializeField] private EPlatformSize platformSize;
    public EPlatformSize PlatformSize => platformSize;

    [Header("Movement Properties")]
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float smoothTimeSpeed = .05f;
    
    private float _refZeroVelocity = .0f;
    private float _yPosition;
    
    [Header("Collisions & Triggers")]
    private readonly List<PlatformTrigger> _pendingTriggersEnter = new();
    
    [Header("Ball Properties")]
    [SerializeField] private GameObject ballCollisionAudioPrefab;
    [SerializeField, Min(.0f)] private float maxAngleOnCorners;

    [Header("Shoot Properties")]
    [SerializeField] private GameObject bulletPrefab; // TODO modifier cette partie
    [SerializeField] private GameObject bulletAudioPrefab; // TODO modifier cette partie
    [SerializeField, Min(.0f)] private float fireRate;
    private float _remainingFireRate;
    

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _animator = GetComponent<Animator>();
        
        _yPosition = transform.position.y;
        
        Assert.IsNotNull(ballCollisionAudioPrefab);
        Assert.IsTrue(ballCollisionAudioPrefab.GetComponent<Audio>());
        
        Assert.IsNotNull(bulletPrefab);
        Assert.IsTrue(bulletPrefab.GetComponent<Bullet>());
        Assert.IsNotNull(bulletAudioPrefab);
        Assert.IsTrue(bulletAudioPrefab.GetComponent<Audio>());
    }

    private void FixedUpdate()
    {
        MovePlatform();
        HandleTriggers();
        FixVerticalVelocity();
        UpdateFire();
    }
    
    /// <summary>
    /// Joue l'animation d'apparition de la plateforme.
    /// </summary>
    public void SpawnPlatform()
    {
        UpdatePlatformSize(PlatformSize);
        Debug.Log($"[Platform / {name}] Start animation");
        _animator.SetTrigger(AnimationTriggerSpawn);
    }

    /// <summary>
    /// Notify Platforms Manager the platform finished his spawn animation.
    /// </summary>
    public void OnReceiveNotificationFromAnimatorPlatformSpawned()
    {
        PlatformsManager.Instance.OnReceivedNotificationFromUnityObject(this);
    }
    
    /// <summary>
    /// Update platform size.
    /// </summary>
    /// <param name="newPlatformSize"></param>
    private void UpdatePlatformSize(EPlatformSize newPlatformSize)
    {
        platformSize = newPlatformSize;
        SOPlatformSize platformSizeProperties = PlatformsManager.Instance.GetPlatformSizeByItsType(platformSize);
        
        _animator.SetInteger(AnimationIntegerPlatformSizeLevel, platformSizeProperties.SizeLevel);
        _boxCollider.size = platformSizeProperties.ColliderSize;
        speed = platformSizeProperties.Speed;
        smoothTimeSpeed = platformSizeProperties.SmoothTime;
    }

    /// <summary>
    /// Returns normalized direction from given position (x).
    /// For example : the ball collides with the platform and needs a reflection.
    /// </summary>
    /// <param name="givenPosition"></param>
    /// <returns></returns>
    public Vector2 GetBallNormalizedDirectionFromGivenPosition(float givenPosition)
    {
        float minCornerPosition = transform.position.x - _boxCollider.size.x / 2;
        float maxCornerPosition = transform.position.x + _boxCollider.size.x / 2;
        
        // On garde la coordonnée dans les bornes
        float clampedGivenPosition = Mathf.Clamp(givenPosition, 
            minCornerPosition,
            maxCornerPosition);

        // Renvoie une valeur entre 0 et 1
        float t = Mathf.InverseLerp(minCornerPosition, maxCornerPosition, clampedGivenPosition);

        // Renvoie l'angle à renvoyer
        float angle = Mathf.Lerp(-maxAngleOnCorners, maxAngleOnCorners, t);
        
        // On part de Vector2.up, il faut donc inverser
        float radians = angle * Mathf.Deg2Rad;
        return new Vector2(
            Mathf.Sin(radians),
            Mathf.Cos(radians)
        );
    }

    /// <summary>
    /// Update platform's velocity, based on player inputs.
    /// </summary>
    private void MovePlatform()
    {
        float currentHorizontalVelocity = _rigidBody.linearVelocityX;
        float targetHorizontalVelocity = PlatformsManager.Instance.inputHorizontalDirection * speed;
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

    private void UpdateFire()
    {
        if (!LevelManager.Instance.LevelState.Equals(ELevelState.GameStarted))
        {
            return;
        }
        
        if (_remainingFireRate.Equals(.0f))
        {
            if (PlatformsManager.Instance.inputFirePressed)
            {
                _remainingFireRate = fireRate;
                Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                Instantiate(bulletAudioPrefab, transform.position, Quaternion.identity);
            }
            return;
        }
        
        _remainingFireRate -= Time.fixedDeltaTime;
        if (_remainingFireRate < .0f)
        {
            _remainingFireRate = .0f;
        }
    }

    /// <summary>
    /// Renvoie la coordonnée du Collider le plus haut pour la ball preview.
    /// </summary>
    /// <param name="collisionPoint"></param>
    /// <returns></returns>
    public Vector2 GetStartPointForBallPreview(Vector2 collisionPoint)
    {
        Bounds bounds = _boxCollider.bounds;

        return new Vector2(
            collisionPoint.x,
            bounds.max.y
        );
    }
    
    /// <summary>
    /// Reçoit une notification d'une nouvelle entrée de trigger à traiter.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="collider2d"></param>
    public void RegisterTriggerEnter(IPlatformTriggerHandler handler, Collider2D collider2d)
    {
        // Un GameObject ne peut entrer en collision qu'une seule fois avec l'objet.
        // Ce code évite le traitement multiple des collisions pour un seul et même GameObject.
        if (_pendingTriggersEnter.Exists(platformTrigger => platformTrigger.Handler.Equals(handler)))
            return;
        
        _pendingTriggersEnter.Add(
            new PlatformTrigger(handler, collider2d));
    }
    
    /// <summary>
    /// Traite les entrées et sorties de trigger entre 2 frames.
    /// Uniquement le trigger d'entrée, rien à faire pour le trigger de sortie.
    /// </summary>
    private void HandleTriggers()
    {
        HandleTriggersEnter();
    }

    /// <summary>
    /// Traite chacune des entrées de trigger entre 2 frames.
    /// </summary>
    private void HandleTriggersEnter()
    {
        if (_pendingTriggersEnter.Count == 0)
            return;

        foreach (PlatformTrigger trigger in _pendingTriggersEnter)
        {
            TriggerResponse _ = trigger.Handler.HandlePlatformTriggerEnter(
                trigger.Collider, this);
        }
        
        _pendingTriggersEnter.Clear();
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        GameObject collidedObject = other.gameObject;

        if (!collidedObject.TryGetComponent(out Ball ball))
            return;
        
        ball.RegisterCollision(this, other);
    }

    public CollisionResponse HandleBallCollision(Collision2D collision, Ball _)
    {
        Instantiate(ballCollisionAudioPrefab, collision.transform.position, Quaternion.identity);
        float ballPosition = collision.gameObject.transform.position.x;
        Vector2 reflectionDirection = GetBallNormalizedDirectionFromGivenPosition(ballPosition);
        return new CollisionResponse(reflectionDirection);
    }
    
}
