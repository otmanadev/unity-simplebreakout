using UnityEngine;

public interface IBallCollisionHandler
{
    
    CollisionResponse HandleBallCollision(Collision2D collision, Ball ball);
    
}