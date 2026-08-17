using UnityEngine;

public class BallCollision
{
    
    public readonly IBallCollisionHandler Handler;
    public readonly Collision2D Collision;

    public BallCollision(IBallCollisionHandler collisionHandler, Collision2D collision)
    {
        Handler = collisionHandler;
        Collision = collision;
    }

}