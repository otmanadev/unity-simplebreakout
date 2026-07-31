using System;
using UnityEngine;

public class DeadArea : Area
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject collidedObject = other.gameObject;

        if (collidedObject.TryGetComponent(out Ball _))
            LevelManager.Instance.OnBallReachedDeadZoneNotification();
    }
    
}
