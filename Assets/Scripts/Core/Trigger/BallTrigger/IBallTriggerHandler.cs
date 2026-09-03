using UnityEngine;

public interface IBallTriggerHandler
{
    
    TriggerResponse HandleBallTriggerEnter(Collider2D collider2d, Ball ball);
    
    TriggerResponse HandleBallTriggerExit(Collider2D collider2d, Ball ball);
    
}