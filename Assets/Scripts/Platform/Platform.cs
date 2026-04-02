using System;
using NUnit.Framework;
using UnityEngine;

public class Platform : MonoBehaviour
{
    
    private Rigidbody2D _rigidBody;

    [Header("Movement")] 
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private float smoothTimeSpeed = .05f;
    private float _inputHorizontalDirection = .0f;
    private float _refZeroVelocity = .0f;
    
    public float InputHorizontalDirection { set => _inputHorizontalDirection = value; }

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        Assert.IsNotNull(_rigidBody);
    }

    private void FixedUpdate()
    {
        MovePlatform();
    }

    /// <summary>
    /// Returns normalized direction from given position.
    /// For example : the ball collides with the platform and needs to be sent back.
    /// </summary>
    /// <param name="givenPosition"></param>
    /// <returns></returns>
    public Vector2 GetNormalizedDirection(Vector2 givenPosition)
    {
        Vector2 currentPosition = new Vector2(transform.position.x, transform.position.y);
        return (givenPosition - currentPosition).normalized;
    }

    /// <summary>
    /// Update platform's velocity, based on player inputs.
    /// </summary>
    private void MovePlatform()
    {
        float currentHorizontalVelocity = _rigidBody.linearVelocityX;
        float targetHorizontalVelocity = _inputHorizontalDirection * speed;
        _rigidBody.linearVelocityX = Mathf.SmoothDamp(currentHorizontalVelocity, targetHorizontalVelocity, ref _refZeroVelocity, smoothTimeSpeed);
    }
    
}
