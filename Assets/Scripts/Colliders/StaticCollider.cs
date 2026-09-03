using NUnit.Framework;
using UnityEngine;

[RequireComponent(typeof(CompositeCollider2D))]
public class StaticCollider : MonoBehaviour, IBallCollisionHandler, IBulletCollisionHandler
{
    
    [Header("Collision Properties")]
    [SerializeField] private GameObject ballCollisionAudioPrefab;
    [SerializeField] private GameObject bulletCollisionAudioPrefab;
    
    private void Awake()
    {
        Assert.IsNotNull(ballCollisionAudioPrefab);
        Assert.IsTrue(ballCollisionAudioPrefab.GetComponent<Audio>());
        
        Assert.IsNotNull(bulletCollisionAudioPrefab);
        Assert.IsTrue(bulletCollisionAudioPrefab.GetComponent<Audio>());
        
        CompositeCollider2D compositeCollider2D = GetComponent<CompositeCollider2D>();
        compositeCollider2D.isTrigger = false;
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
        Instantiate(ballCollisionAudioPrefab, collision.transform.position, Quaternion.identity);
        Vector2 normal = Vector2.zero;

        for (int i = 0; i < collision.contactCount; i++)
        {
            normal += collision.GetContact(i).normal;
        }

        normal.Normalize();

        Vector2 reflectedDirection =  Vector2.Reflect(ball.Movement.Direction, normal);
        return new CollisionResponse(reflectedDirection);
    }

    public CollisionResponse HandleBulletCollision(Collision2D collision, Bullet _)
    {
        Instantiate(bulletCollisionAudioPrefab, collision.transform.position, Quaternion.identity);
        return new CollisionResponse();
    }
    
}
