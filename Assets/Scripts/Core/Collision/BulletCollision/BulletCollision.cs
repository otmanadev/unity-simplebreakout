using UnityEngine;

public class BulletCollision
{
    
    public readonly IBulletCollisionHandler Handler;
    public readonly Collision2D Collision;

    public BulletCollision(IBulletCollisionHandler collisionHandler, Collision2D collision)
    {
        Handler = collisionHandler;
        Collision = collision;
    }

}