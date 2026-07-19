using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DeadZoneTrigger : MonoBehaviour
{
    private void Awake()
    {
        Collider2D collider2D = GetComponent<Collider2D>();
        
        if (!collider2D.isTrigger)
        {
            collider2D.isTrigger = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("collision");
        Debug.Log(collision.gameObject.name);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("trigger");
        Debug.Log(other.name);
        GameObject collidedObject = other.gameObject;

        if (collidedObject.TryGetComponent(out Ball _))
        {
            LevelManager.Instance.OnBallReachedDeadZoneNotification();
        }
    }
}
