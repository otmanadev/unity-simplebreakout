using UnityEngine;

public interface IPlatformTriggerHandler
{
    
    TriggerResponse HandlePlatformTriggerEnter(Collider2D collider2d, Platform platform);
    
    TriggerResponse HandlePlatformTriggerExit(Collider2D collider2d, Platform platform);
    
}