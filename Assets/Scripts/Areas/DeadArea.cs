using System;
using UnityEngine;

public class DeadArea : Area
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        Debug.Log("OnCollisionEnter2D");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject collidedObject = other.gameObject;
        
        Debug.Log("OnTriggerEnter2D");

        if (collidedObject.TryGetComponent(out Ball _))
            LevelManager.Instance.OnBallReachedDeadZoneNotification();
    }
    
}
