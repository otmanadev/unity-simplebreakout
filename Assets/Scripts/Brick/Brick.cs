using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Animator))]
public class Brick : MonoBehaviour, IBallCollisionHandler, IBulletCollisionHandler
{

    private static readonly String AnimationTriggerSpawn = "SpawnTrigger";
    
    [Header("References")]
    private BoxCollider2D _boxCollider2D;
    private Animator _animator;
    [SerializeField] private SpriteRenderer outlineSpriteRenderer;
    [SerializeField] private SpriteRenderer backgroundSpriteRenderer;
    
    private List<IBrickPassive> _brickPassives = new List<IBrickPassive>();
    public List<IBrickPassive> BrickPassives => _brickPassives;
    
    [Header("Attached Power Up")]
    [SerializeField] private SOPowerUp powerUp;

    [Header("Health")]
    [SerializeField, Min(0)] private int health;
    [SerializeField, Min(.0f)] private float hitDuration;
    [SerializeField] private Color backgroundColorWhenHit;
    [SerializeField] private Color defaultBackgroundColor;
    private Coroutine _coroutineDamageColor = null;
    
    [Header("Collision Properties")]
    [SerializeField] private GameObject brickHitAudioPrefab;
    [SerializeField] private GameObject brickDestroyedAudioPrefab;
    
    protected virtual void Awake()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _animator = GetComponent<Animator>();
        
        Assert.IsNotNull(outlineSpriteRenderer);
        Assert.IsNotNull(backgroundSpriteRenderer);
        
        Assert.IsNotNull(brickHitAudioPrefab);
        Assert.IsTrue(brickHitAudioPrefab.GetComponent<Audio>());
        
        Assert.IsNotNull(brickDestroyedAudioPrefab);
        Assert.IsTrue(brickDestroyedAudioPrefab.GetComponent<Audio>());

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
            outlineSpriteRenderer.material = material;
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

        if (health > 0)
        {
            Instantiate(brickHitAudioPrefab, transform.position, Quaternion.identity);
            if (_coroutineDamageColor != null)
                StopCoroutine(_coroutineDamageColor);
            _coroutineDamageColor = StartCoroutine(DamageColor());
            foreach (IBrickPassive brickPassive in _brickPassives)
            {
                brickPassive.OnBrickHurt();
            }
            return;
        }
        
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
    /// Modifie la couleur de la brique de fond lorsqu'il subit des dommages.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DamageColor()
    {
        float elapsedTime = .0f;

        while (elapsedTime < hitDuration)
        {
            float t = elapsedTime / hitDuration;
            
            backgroundSpriteRenderer.color = Color.Lerp(backgroundColorWhenHit, defaultBackgroundColor, t);
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        backgroundSpriteRenderer.color = defaultBackgroundColor;
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
        
        if (collidedObject.TryGetComponent(out Ball ball))
            ball.RegisterCollision(this, other);
        
        if (collidedObject.TryGetComponent(out Bullet bullet))
            bullet.RegisterCollision(this, other);
    }

    public CollisionResponse HandleBallCollision(Collision2D collision, Ball ball)
    {
        Vector2 normal = collision.GetContact(0).normal;
        Vector2 reflectedDirection = Vector2.Reflect(ball.Movement.Direction, normal);
        
        TryHitBrick(ball.Damage);
        return new CollisionResponse(reflectedDirection);
    }

    public CollisionResponse HandleBulletCollision(Collision2D collision, Bullet bullet)
    {
        TryHitBrick(bullet.Damage);
        return new CollisionResponse();
    }
}
