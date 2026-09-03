using UnityEngine;

public interface IBulletCollisionHandler
{
    
    CollisionResponse HandleBulletCollision(Collision2D collision, Bullet bullet);
    
}