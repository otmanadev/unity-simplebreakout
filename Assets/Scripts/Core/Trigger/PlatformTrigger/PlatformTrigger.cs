using UnityEngine;

public class PlatformTrigger
{
    
    public readonly IPlatformTriggerHandler Handler;
    public readonly Collider2D Collider;

    public PlatformTrigger(IPlatformTriggerHandler collisionHandler, Collider2D collider)
    {
        Handler = collisionHandler;
        Collider = collider;
    }

}