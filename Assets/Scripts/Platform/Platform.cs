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
    private bool _canMoveToRight = true;
    private bool _canMoveToLeft = true;
    
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        GameObject collidedObject = other.gameObject;

        if (collidedObject.TryGetComponent(out StaticCollider _))
        {
            if (_rigidBody.linearVelocityX > .0f)
            {
                _canMoveToRight = false;
            }
            
            if (_rigidBody.linearVelocityX < .0f)
            {
                _canMoveToLeft = false;
            }
        }
    }
    
    private void OnCollisionExit2D(Collision2D other)
    {
        GameObject collidedObject = other.gameObject;

        if (collidedObject.TryGetComponent(out StaticCollider _))
        {
            if (_rigidBody.linearVelocityX > .0f)
            {
                _canMoveToLeft = true;
            }
            
            if (_rigidBody.linearVelocityX < .0f)
            {
                _canMoveToRight = true;
            }
        }
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
        float currentInputHorizontalDirection = _inputHorizontalDirection;

        if ((currentInputHorizontalDirection < .0f && !_canMoveToLeft) || (currentInputHorizontalDirection > .0f && !_canMoveToRight))
        {
            currentInputHorizontalDirection = .0f;
        }
        
        float currentHorizontalVelocity = _rigidBody.linearVelocityX;
        float targetHorizontalVelocity = currentInputHorizontalDirection * speed;
        _rigidBody.linearVelocityX = Mathf.SmoothDamp(currentHorizontalVelocity, targetHorizontalVelocity, ref _refZeroVelocity, smoothTimeSpeed);
    }
    
}
