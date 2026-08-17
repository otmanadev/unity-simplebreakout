using UnityEngine;

public interface IBallCollisionHandler
{
    
    CollisionResponse HandleBallCollision(Collision2D collision, Vector2 ballDirection);
    
}