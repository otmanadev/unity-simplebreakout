using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class Brick : MonoBehaviour
{

    private static readonly String AnimationTriggerSpawn = "SpawnTrigger";
    
    private BoxCollider2D _boxCollider2D;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    
    [Header("Attached Power Up")]
    [SerializeField] private SOPowerUp powerUp;

    [Header("Health")]
    [SerializeField, Min(0)] private int health;
    
    protected virtual void Awake()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        if (powerUp != null)
        {
            Debug.Log($"[Brick / {name}] Brick has attached power up {powerUp.Type}");
            SetUpPowerUp(powerUp);
        }
        Debug.Log($"[Brick / {name}] Send notification to Bricks Manager : <color=orange>Brick initialized</color>");
        BricksManager.Instance.OnBrickInitializedNotification(this);
    }

    /// <summary>
    /// Play brick spawn animation.
    /// </summary>
    public void StartBrickSpawn()
    {
        Debug.Log($"[Brick / {name}] Start animation");
        _animator.SetTrigger(AnimationTriggerSpawn);
    }

    /// <summary>
    /// Notify Bricks Manager the brick finished his spawn animation.
    /// </summary>
    public void OnReceiveNotificationFromAnimatorBrickSpawned()
    {
        Debug.Log($"[Brick / {name}] Send notification to Bricks Manager : <color=orange>Brick spawned</color>");
        BricksManager.Instance.OnBrickSpawnedNotification(this);
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
            return;
        
        Debug.Log($"[Brick / {name}] Send notification to {BricksManager.Instance.name} : Brick destroyed.");
        BricksManager.Instance.OnBrickDestroyedNotification(this);
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
    
}
