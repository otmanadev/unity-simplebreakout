using NUnit.Framework;
using UnityEngine;

[RequireComponent(typeof(CompositeCollider2D))]
public class StaticCollider : MonoBehaviour, IBallCollisionHandler
{
    
    [Header("Ball Properties")]
    [SerializeField] private GameObject ballCollisionAudioPrefab;
    
    private void Awake()
    {
        Assert.IsNotNull(ballCollisionAudioPrefab);
        Assert.IsTrue(ballCollisionAudioPrefab.GetComponent<Audio>());
        
        CompositeCollider2D compositeCollider2D = GetComponent<CompositeCollider2D>();
        compositeCollider2D.isTrigger = false;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        GameObject collidedObject = other.gameObject;

        if (!collidedObject.TryGetComponent(out Ball ball))
            return;
        
        ball.RegisterCollision(this, other);
    }

    public CollisionResponse HandleBallCollision(Collision2D collision, Vector2 ballDirection)
    {
        Instantiate(ballCollisionAudioPrefab, collision.transform.position, Quaternion.identity);
        Vector2 normal = collision.GetContact(0).normal;
        Vector2 reflectedDirection = Vector2.Reflect(ballDirection, normal);
        return new CollisionResponse(reflectedDirection);
    }
    
}
