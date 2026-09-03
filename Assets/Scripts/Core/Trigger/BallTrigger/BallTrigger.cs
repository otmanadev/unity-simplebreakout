using UnityEngine;

public class BallTrigger
{
    
    public readonly IBallTriggerHandler Handler;
    public readonly Collider2D Collider;

    public BallTrigger(IBallTriggerHandler collisionHandler, Collider2D collider)
    {
        Handler = collisionHandler;
        Collider = collider;
    }

}