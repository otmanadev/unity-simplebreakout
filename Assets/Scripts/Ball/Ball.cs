using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class Ball : MonoBehaviour
{
    
    private static readonly String AnimationTriggerSpawn = "SpawnTrigger";
    private static readonly String AnimationTriggerBallBigger = "BallBiggerTrigger";
    private static readonly String AnimationTriggerBallSmaller = "BallSmallerTrigger";
    private static readonly String AnimationIntegerBallSizeLevel = "BallSizeLevel";
    private static readonly String AnimationTriggerDespawn = "DespawnTrigger";
    
    private Rigidbody2D _rigidBody;
    private CircleCollider2D _circleCollider;
    private Animator _animator;

    [SerializeField] private EBallSize ballSize;
    public EBallSize BallSize => ballSize;

    [Header("Damage")] 
    [SerializeField, Min(1)] private int damage;
    public int Damage => damage;

    private bool _hasReflection;
    private Collision2D _collidedGameObject;
    
    [Header("Movement")]
    [SerializeField] private Movement movement;
    public Movement Movement => movement;
    
    private Vector2 _direction;
    public Vector2 Direction => _direction;
    private Vector2 _refZeroVelocity = Vector2.zero;
    
    [Header("Collisions & Triggers")]
    private readonly List<BallCollision> _pendingCollisions = new();
    private readonly List<BallTrigger> _pendingTriggersEnter = new();
    private readonly List<BallTrigger> _pendingTriggersExit = new();
    
    [Header("Hit properties")]
    [SerializeField] private GameObject hitPlatformAudioPrefab;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _circleCollider = GetComponent<CircleCollider2D>();
        _animator = GetComponent<Animator>();
        
        Assert.IsNotNull(hitPlatformAudioPrefab);
        Assert.IsTrue(hitPlatformAudioPrefab.GetComponent<Audio>());
        
        _circleCollider.enabled = false;
    }

    private void FixedUpdate()
    {
        HandleCollisions();
        HandleTriggers();
        UpdateBallVelocity();
    }

    /// <summary>
    /// Start move ball.
    /// </summary>
    public void StartMoveBall()
    {
        _circleCollider.enabled = true;
    }
    
    /// <summary>
    /// Met à jour la vélocité de la balle en fonction de sa direction et des vitesses appliquées à la balle.
    /// </summary>
    private void UpdateBallVelocity()
    {
        Vector2 currentVelocity = _rigidBody.linearVelocity;
        Vector2 targetVelocity = BallsManager.Instance.MovementMultiplier * movement.GetMovementDirection;
        _rigidBody.linearVelocity = Vector2.SmoothDamp(currentVelocity, targetVelocity, ref _refZeroVelocity, .0f);
    }

    // ///////////////////////////////////////////////////////////////
    // COLLISIONS & TRIGGERS
    // ///////////////////////////////////////////////////////////////
    
    /// <summary>
    /// Reçoit une notification d'une nouvelle collision à traiter.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="collision"></param>
    public void RegisterCollision(IBallCollisionHandler handler, Collision2D collision)
    {
        // Un GameObject ne peut entrer en collision qu'une seule fois avec l'objet.
        // Ce code évite le traitement multiple des collisions pour un seul et même GameObject.
        if (_pendingCollisions.Exists(ballCollision => ballCollision.Handler.Equals(handler)))
            return;
        
        _pendingCollisions.Add(
            new BallCollision(handler, collision));
    }

    /// <summary>
    /// Traite chacune des collisions enregistrées entre 2 frames.
    /// </summary>
    private void HandleCollisions()
    {
        if (_pendingCollisions.Count == 0)
             return;

        Vector2 ballDirection = Vector2.zero;
        foreach (BallCollision collision in _pendingCollisions)
        {
            CollisionResponse response = collision.Handler.HandleBallCollision(
                collision.Collision, this);

            ballDirection += response.ReflectedDirection;
        }
        Movement.UpdateDirectionNormalized(ballDirection);
        
        _pendingCollisions.Clear();
    }
    
    /// <summary>
    /// Reçoit une notification d'une nouvelle entrée de trigger à traiter.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="collider2d"></param>
    public void RegisterTriggerEnter(IBallTriggerHandler handler, Collider2D collider2d)
    {
        // Un GameObject ne peut entrer en collision qu'une seule fois avec l'objet.
        // Ce code évite le traitement multiple des collisions pour un seul et même GameObject.
        if (_pendingTriggersEnter.Exists(ballTrigger => ballTrigger.Handler.Equals(handler)))
            return;
        
        _pendingTriggersEnter.Add(
            new BallTrigger(handler, collider2d));
    }
    
    /// <summary>
    /// Reçoit une notification d'une nouvelle sortie de trigger à traiter.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="collider2d"></param>
    public void RegisterTriggerExit(IBallTriggerHandler handler, Collider2D collider2d)
    {
        // Un GameObject ne peut entrer en collision qu'une seule fois avec l'objet.
        // Ce code évite le traitement multiple des collisions pour un seul et même GameObject.
        if (_pendingTriggersExit.Exists(ballTrigger => ballTrigger.Handler.Equals(handler)))
            return;
        
        _pendingTriggersExit.Add(
            new BallTrigger(handler, collider2d));
    }

    /// <summary>
    /// Traite les entrées et sorties de trigger entre 2 frames.
    /// </summary>
    private void HandleTriggers()
    {
        HandleTriggersEnter();
        HandleTriggersExit();
    }

    /// <summary>
    /// Traite chacune des entrées de trigger entre 2 frames.
    /// </summary>
    private void HandleTriggersEnter()
    {
        if (_pendingTriggersEnter.Count == 0)
            return;

        foreach (BallTrigger trigger in _pendingTriggersEnter)
        {
            TriggerResponse _ = trigger.Handler.HandleBallTriggerEnter(
                trigger.Collider, this);
        }
        
        _pendingTriggersEnter.Clear();
    }
    
    /// <summary>
    /// Traite chacune des sorties de trigger entre 2 frames.
    /// </summary>
    private void HandleTriggersExit()
    {
        if (_pendingTriggersExit.Count == 0)
            return;

        foreach (BallTrigger trigger in _pendingTriggersExit)
        {
            TriggerResponse _ = trigger.Handler.HandleBallTriggerExit(
                trigger.Collider, this);
        }
        
        _pendingTriggersExit.Clear();
    }

    // ///////////////////////////////////////////////////////////////
    // SPAWN
    // ///////////////////////////////////////////////////////////////
    
    /// <summary>
    /// Fait apparaitre la balle.
    /// </summary>
    public void SpawnBall()
    {
        UpdateBallSize(BallSize);
        Debug.Log($"[Ball / {name}] Start animation");
        _animator.SetTrigger(AnimationTriggerSpawn);
    }
    
    /// <summary>
    /// Notifie Level Step que l'apparition de la balle s'est finalisée.
    /// </summary>
    public void OnReceiveNotificationFromAnimatorBallSpawned()
    {
        BallsManager.Instance.OnReceivedNotificationFromUnityObject(this);
    }
    
    // ///////////////////////////////////////////////////////////////
    // DESPAWN
    // ///////////////////////////////////////////////////////////////
    
    /// <summary>
    /// Despawn ball.
    /// </summary>
    public void DespawnBall()
    {
        _circleCollider.enabled = false;
        movement.UpdateDirectionNormalized(Vector2.zero);
        _animator.SetTrigger(AnimationTriggerDespawn);
    }
    
    /// <summary>
    /// Notify Balls Manager the ball finished his despawn animation.
    /// </summary>
    public void OnReceiveNotificationFromAnimatorBallDespawned()
    {
        BallsManager.Instance.OnReceivedNotificationFromUnityObject(this);
        Destroy(gameObject);
    }
    
    // ///////////////////////////////////////////////////////////////
    // BALL SIZE
    // ///////////////////////////////////////////////////////////////
    
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

    /// <summary>
    /// Update ball size.
    /// </summary>
    /// <param name="newBallSizeType"></param>
    private void UpdateBallSize(EBallSize newBallSizeType)
    {
        ballSize = newBallSizeType;
        SOBallSize ballSizeProperties = BallsManager.Instance.GetBallSizeByItsType(BallSize);
        
        _animator.SetInteger(AnimationIntegerBallSizeLevel, ballSizeProperties.SizeLevel);
        _circleCollider.radius = ballSizeProperties.ColliderRadius;
        damage = ballSizeProperties.Damage;
    }
    
}
