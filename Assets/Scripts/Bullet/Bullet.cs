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
    [SerializeField, Min(1)] private int damage;
    public int Damage => damage;

    [Header("Collisions")]
    private readonly List<BulletCollision> _pendingCollisions = new();
    private readonly List<BulletTrigger> _pendingTriggersEnter = new();
    private readonly List<BulletTrigger> _pendingTriggersExit = new();

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        movement.UpdateDirectionNormalized(Vector2.up);
    }

    private void FixedUpdate()
    {
        MoveBullet();
        HandleCollisions();
        HandleTriggers();
    }
    
    private void MoveBullet()
    {
        Vector2 currentVelocity = _rigidBody.linearVelocity;
        Vector2 targetVelocity = movement.GetMovementDirection;
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
    public void RegisterCollision(IBulletCollisionHandler handler, Collision2D collision)
    {
        // Un GameObject ne peut entrer en collision qu'une seule fois avec l'objet.
        // Ce code évite le traitement multiple des collisions pour un seul et même GameObject.
        if (_pendingCollisions.Exists(ballCollision => ballCollision.Handler.Equals(handler)))
            return;
        
        _pendingCollisions.Add(
            new BulletCollision(handler, collision));
    }

    /// <summary>
    /// Traite chacune des collisions enregistrées entre 2 frames.
    /// </summary>
    private void HandleCollisions()
    {
        if (_pendingCollisions.Count == 0)
            return;

        foreach (BulletCollision collision in _pendingCollisions)
        {
            collision.Handler.HandleBulletCollision(collision.Collision, this);
        }
        
        _pendingCollisions.Clear();
        Destroy(gameObject);
    }

    /// <summary>
    /// Reçoit une notification d'une nouvelle entrée de trigger à traiter.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="collider2d"></param>
    public void RegisterTriggerEnter(IBulletTriggerHandler handler, Collider2D collider2d)
    {
        // Un GameObject ne peut entrer en collision qu'une seule fois avec l'objet.
        // Ce code évite le traitement multiple des collisions pour un seul et même GameObject.
        if (_pendingTriggersEnter.Exists(ballTrigger => ballTrigger.Handler.Equals(handler)))
            return;
        
        _pendingTriggersEnter.Add(
            new BulletTrigger(handler, collider2d));
    }
    
    /// <summary>
    /// Reçoit une notification d'une nouvelle sortie de trigger à traiter.
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="collider2d"></param>
    public void RegisterTriggerExit(IBulletTriggerHandler handler, Collider2D collider2d)
    {
        // Un GameObject ne peut entrer en collision qu'une seule fois avec l'objet.
        // Ce code évite le traitement multiple des collisions pour un seul et même GameObject.
        if (_pendingTriggersExit.Exists(ballTrigger => ballTrigger.Handler.Equals(handler)))
            return;
        
        _pendingTriggersExit.Add(
            new BulletTrigger(handler, collider2d));
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

        foreach (BulletTrigger trigger in _pendingTriggersEnter)
        {
            TriggerResponse _ = trigger.Handler.HandleBulletTriggerEnter(
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

        foreach (BulletTrigger trigger in _pendingTriggersExit)
        {
            TriggerResponse _ = trigger.Handler.HandleBulletTriggerExit(
                trigger.Collider, this);
        }
        
        _pendingTriggersExit.Clear();
    }
    
}
