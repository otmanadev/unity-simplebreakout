using System;
using UnityEngine;

public class Ball : MonoBehaviour
{
    private Vector2 direction;
    [SerializeField] private float speed;
    [SerializeField] private float smoothTime;
    
    Vector2 zeroVelocity = Vector2.zero;

    private void Start()
    {
        direction = Vector2.down;
    }

    private void Update()
    {
        MoveBall();
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        GameObject colliderGameObject = other.gameObject;
        if (colliderGameObject.GetComponent<Platform>() != null)
        {
            Debug.Log("Contact with platform");
            direction = (colliderGameObject.transform.position - transform.position).normalized;
        }
        else
        {
            Debug.Log("Contact with something else...");
            ContactPoint2D contact = other.GetContact(0);
            
            Vector2 normal = contact.normal;
            
            direction = Vector2.Reflect(direction, normal).normalized;
        }
    }

    private void MoveBall()
    {
        Vector2 initialPosition = transform.position;
        
        Vector2 targetPosition = initialPosition + direction * speed;
        
        Vector2 calculatedPosition = Vector2.SmoothDamp(initialPosition, targetPosition, ref zeroVelocity, smoothTime);
        
        transform.position = calculatedPosition;
    }
    
}
