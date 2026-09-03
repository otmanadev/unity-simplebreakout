using UnityEngine;

public class BulletTrigger
{
    
    public readonly IBulletTriggerHandler Handler;
    public readonly Collider2D Collider;

    public BulletTrigger(IBulletTriggerHandler collisionHandler, Collider2D collider)
    {
        Handler = collisionHandler;
        Collider = collider;
    }

}