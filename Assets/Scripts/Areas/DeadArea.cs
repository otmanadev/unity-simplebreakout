using System;
using UnityEngine;

public class DeadArea : Area, IBallTriggerHandler, IBulletTriggerHandler
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject collidedObject = other.gameObject;

        if (collidedObject.TryGetComponent(out Ball ball))
            ball.RegisterTriggerEnter(this, other);
        
        if (collidedObject.TryGetComponent(out Bullet bullet))
            bullet.RegisterTriggerEnter(this, other);
    }

    public TriggerResponse HandleBallTriggerEnter(Collider2D _, Ball __)
    {
        LevelManager.Instance.OnBallReachedDeadZoneNotification();
        return new TriggerResponse();
    }

    /// <summary>
    /// Aucune implémentation prévue à cet effet.
    /// </summary>
    /// <param name="_"></param>
    /// <param name="__"></param>
    /// <returns></returns>
    public TriggerResponse HandleBallTriggerExit(Collider2D _, Ball __)
    {
        return new TriggerResponse();
    }

    public TriggerResponse HandleBulletTriggerEnter(Collider2D _, Bullet bullet)
    {
        Destroy(bullet.gameObject);
        return new TriggerResponse();
    }

    /// <summary>
    /// Aucune implémentation prévue à cet effet.
    /// </summary>
    /// <param name="_"></param>
    /// <param name="__"></param>
    /// <returns></returns>
    public TriggerResponse HandleBulletTriggerExit(Collider2D _, Bullet __)
    {
        return new TriggerResponse();
    }
    
}
