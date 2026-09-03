using UnityEngine;

public interface IBulletTriggerHandler
{
    
    TriggerResponse HandleBulletTriggerEnter(Collider2D collider2d, Bullet bullet);
    
    TriggerResponse HandleBulletTriggerExit(Collider2D collider2d, Bullet bullet);
    
}