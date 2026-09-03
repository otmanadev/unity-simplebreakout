using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BlockingArea : Area, IBallCollisionHandler, IBulletCollisionHandler, IBallTriggerHandler, IBulletTriggerHandler
{
    
    private readonly List<int> _triggeredBalls = new();
    private readonly List<Bullet> _triggeredBullets = new();
    
    [Header("Collision Properties")]
    [SerializeField] private GameObject ballCollisionAudioPrefab;
    [SerializeField] private GameObject bulletCollisionAudioPrefab;

    protected override void Awake()
    {
        base.Awake();
        
        Assert.IsNotNull(ballCollisionAudioPrefab);
        Assert.IsTrue(ballCollisionAudioPrefab.GetComponent<Audio>());
        
        Assert.IsNotNull(ballCollisionAudioPrefab);
        Assert.IsTrue(ballCollisionAudioPrefab.GetComponent<Audio>());
        
        CompositeCollider2D.enabled = true;
        TilemapCollider2D.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        GameObject collidedObject = other.gameObject;

        if (collidedObject.TryGetComponent(out Ball ball))
            ball.RegisterCollision(this, other);

        if (collidedObject.TryGetComponent(out Bullet bullet))
            bullet.RegisterCollision(this, other);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject collidedObject = other.gameObject;
 
        if (collidedObject.TryGetComponent(out Ball ball))
            ball.RegisterTriggerEnter(this, other);
 
        if (collidedObject.TryGetComponent(out Bullet bullet))
            bullet.RegisterTriggerEnter(this, other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        GameObject collidedObject = other.gameObject;
        
        if (collidedObject.TryGetComponent(out Ball ball))
            ball.RegisterTriggerExit(this, other);
 
        if (collidedObject.TryGetComponent(out Bullet bullet))
            bullet.RegisterTriggerExit(this, other);
    }

    public override void EnableArea()
    {
        isActive = true;
        CheckIfAreaCanBeEnabled();
    }

    public override void DisableArea()
    {
        isActive = false;
        CompositeCollider2D.isTrigger = true;
        areaUI.SetActive(false);
        UpdateAreaColor();
    }

    /// <summary>
    /// Vérifie que l'on peut activer l'Area si les conditions sont respectées :
    /// - Est censé être actif
    /// - N'a aucune balle dans la zone
    /// </summary>
    private void CheckIfAreaCanBeEnabled()
    {
        if (!isActive)
            return;

        if (_triggeredBalls.Count > 0)
            return;
        
        foreach (Bullet bullet in _triggeredBullets)
            Destroy(bullet.gameObject);
        _triggeredBullets.Clear();
        
        CompositeCollider2D.isTrigger = false;
        areaUI.SetActive(true);
        UpdateAreaColor();
    }

    public CollisionResponse HandleBallCollision(Collision2D collision, Ball ball)
    {
        Instantiate(ballCollisionAudioPrefab, collision.transform.position, Quaternion.identity);
        Vector2 normal = collision.GetContact(0).normal;
        Vector2 reflectedDirection = Vector2.Reflect(ball.Movement.Direction, normal);
        return new CollisionResponse(reflectedDirection);
    }

    public CollisionResponse HandleBulletCollision(Collision2D collision, Bullet _)
    {
        Instantiate(ballCollisionAudioPrefab, collision.transform.position, Quaternion.identity);
        return new CollisionResponse();
    }

    public TriggerResponse HandleBallTriggerEnter(Collider2D _, Ball ball)
    {
        int ballInstance = ball.GetInstanceID();
        if (!_triggeredBalls.Contains(ballInstance))
            _triggeredBalls.Add(ballInstance);
        return new TriggerResponse();
    }

    public TriggerResponse HandleBallTriggerExit(Collider2D _, Ball ball)
    {
        int ballInstance = ball.GetInstanceID();
        if (_triggeredBalls.Contains(ballInstance))
        {
            _triggeredBalls.Remove(ballInstance);
            CheckIfAreaCanBeEnabled();
        }
        return new TriggerResponse();
    }

    public TriggerResponse HandleBulletTriggerEnter(Collider2D _, Bullet bullet)
    {
        if (!_triggeredBullets.Contains(bullet))
            _triggeredBullets.Add(bullet);
        return new TriggerResponse();
    }

    public TriggerResponse HandleBulletTriggerExit(Collider2D _, Bullet bullet)
    {
        if (_triggeredBullets.Contains(bullet))
            _triggeredBullets.Remove(bullet);
        return new TriggerResponse();
    }
    
}
