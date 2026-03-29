using System;
using UnityEngine;

public class Platform : MonoBehaviour
{

    public float inputDirection = .0f;

    [Header("Movement Configuration")] 
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private float smoothTimeSpeed = 1.0f;

    private Vector2 zeroVelocity = Vector2.zero;


    private void FixedUpdate()
    {
        UpdateMovement();
    }

    private void UpdateMovement()
    {
        Vector2 initialPosition = transform.position;
        
        Vector2 targetPosition = initialPosition + inputDirection * moveSpeed * Vector2.right;
        
        Vector2 calculatedPosition = Vector2.SmoothDamp(initialPosition, targetPosition, ref zeroVelocity, smoothTimeSpeed);
        
        transform.position = calculatedPosition;
    }
    
}
