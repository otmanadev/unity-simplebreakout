using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class Brick : MonoBehaviour, IBallCollisionHandler
{

    private static readonly String AnimationTriggerSpawn = "SpawnTrigger";
    
    private BoxCollider2D _boxCollider2D;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    
    private List<IBrickPassive> _brickPassives = new List<IBrickPassive>();
    public List<IBrickPassive> BrickPassives => _brickPassives;
    
    [Header("Attached Power Up")]
    [SerializeField] private SOPowerUp powerUp;

    [Header("Health")]
    [SerializeField, Min(0)] private int health;
    [SerializeField] private GameObject brickHitAudioPrefab;
    [SerializeField] private GameObject brickDestroyedAudioPrefab;
    
    [Header("Ball Properties")]
    [SerializeField] private GameObject ballCollisionAudioPrefab;
    
    protected virtual void Awake()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        Assert.IsNotNull(brickHitAudioPrefab);
        Assert.IsTrue(brickHitAudioPrefab.GetComponent<Audio>());
        
        Assert.IsNotNull(brickDestroyedAudioPrefab);
        Assert.IsTrue(brickDestroyedAudioPrefab.GetComponent<Audio>());
        
        Assert.IsNotNull(ballCollisionAudioPrefab);
        Assert.IsTrue(ballCollisionAudioPrefab.GetComponent<Audio>());

        CheckHasAttachedPowerUp();
    }

    private void CheckHasAttachedPowerUp()
    {
        if (powerUp == null)
            return;
        
        Debug.Log($"[Brick / {name}] Brick has attached power up {powerUp.Type}");
        SetUpPowerUp(powerUp);
    }

    /// <summary>
    /// Joue l'animation d'apparition de la brique.
    /// </summary>
    public void StartBrickSpawn()
    {
        Debug.Log($"[Brick / {name}] Start animation");
        _animator.SetTrigger(AnimationTriggerSpawn);
    }

    /// <summary>
    /// Notifie Level Step que l'apparition de la brique s'est finalisée.
    /// </summary>
    public void OnReceiveNotificationFromAnimatorBrickSpawned()
    {
        BricksManager.Instance.OnReceivedNotificationFromUnityObject(this);
    }
    
    /// <summary>
    /// Receive given power up.
    /// </summary>
    /// <param name="givenPowerUp"></param>
    public void SetUpPowerUp(SOPowerUp givenPowerUp)
    {
        powerUp = givenPowerUp;
        if (powerUp.BrickMaterial != null)
        {
            Material material = new Material(powerUp.BrickMaterial);
            _spriteRenderer.material = material;
        }
    }

    /// <summary>
    /// Hit brick with ball damages.
    /// If brick has no health points left, he is destroyed.
    /// </summary>
    /// <param name="givenDamage"></param>
    public void TryHitBrick(int givenDamage)
    {
        health -= givenDamage;
        Debug.Log($"[Brick / {name}] Received {givenDamage} damage. Now has {health} health point(s) left.");

        if (health > 0)
        {
            Instantiate(brickHitAudioPrefab, transform.position, Quaternion.identity);
            foreach (IBrickPassive brickPassive in _brickPassives)
            {
                brickPassive.OnBrickHurt();
            }
            return;
        }
        
        Debug.Log($"[Brick / {name}] Send notification to {BricksManager.Instance.name} : Brick destroyed.");
        Instantiate(brickDestroyedAudioPrefab, transform.position, Quaternion.identity);
        BricksManager.Instance.OnBrickDestroyedNotification(this);
        foreach (IBrickPassive brickPassive in _brickPassives)
        {
            brickPassive.OnBrickDeactivated();
        }
        SpawnAttachedPowerUp();
        Destroy(gameObject);
    }

    /// <summary>
    /// Appears power up if attached.
    /// </summary>
    private void SpawnAttachedPowerUp()
    {
        if (powerUp == null)
            return;
        
        Debug.Log($"[Brick / {name}] Notify Power Ups Manager to spawn throwing Power Up from type {powerUp.Type} in given coordinates {transform.position}");
        PowerUpsManager.Instance.SpawnThrowingPowerUp(powerUp, transform.position);
    }

    public void NotifyBrickPassives()
    {
        foreach (IBrickPassive brickPassive in _brickPassives)
        {
            brickPassive.OnBrickActivated();
        }
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        GameObject collidedObject = other.gameObject;

        if (!collidedObject.TryGetComponent(out Ball ball))
            return;
        
        Debug.Log($"[BRICK - {name}] Collision avec la balle aux coordonnées : " + collidedObject.transform.position);
        ball.RegisterCollision(this, other);
    }

    public CollisionResponse HandleBallCollision(Collision2D collision, Vector2 ballDirection)
    {
        Instantiate(ballCollisionAudioPrefab, collision.transform.position, Quaternion.identity);
        Vector2 normal = collision.GetContact(0).normal;
        Vector2 reflectedDirection = Vector2.Reflect(ballDirection, normal);
        
        TryHitBrick(100);
        return new CollisionResponse(reflectedDirection);
    }
    
}
