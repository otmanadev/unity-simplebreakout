using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[RequireComponent(typeof(Brick))]
public class MovingBrick : MonoBehaviour, IBrickPassive
{

    private Brick _brick;
    
    [Header("Movement Properties")]
    [SerializeField, Min(0f)] private float moveSpeed;
    [SerializeField] private List<Transform> coordinates;
    private float _currentMoveSpeed;
    private int _indexCoordinates;
    private bool _isMoving = false;
    Vector2 _zeroVelocity = Vector2.zero;
    
    [Header("Hurt Properties")]
    [SerializeField, Min(0f)] private float hurtTime;
    [SerializeField, Min(0f)] private float minMoveSpeedWhenHurted;
    private float _currentHurtTime;

    private void Awake()
    {
        Assert.IsNotNull(coordinates);
        Assert.IsTrue(coordinates.Count > 1);
        
        _brick = GetComponent<Brick>();
        _brick.BrickPassives.Add(this);
    }

    private void FixedUpdate()
    {
        UpdateMoveSpeed();
        MoveBrick();
    }

    private void MoveBrick()
    {
        if (!_isMoving)
            return;
        
        float step = _currentMoveSpeed * Time.fixedDeltaTime;
        Vector2 targetPosition = coordinates[_indexCoordinates].position;
        Vector2 currentPosition = _brick.transform.position;

        _brick.transform.position = Vector2.MoveTowards(currentPosition, targetPosition, step);

        if (Vector2.Distance(_brick.transform.position, targetPosition) < 0.1f)
        {
            _indexCoordinates = (_indexCoordinates + 1) % coordinates.Count;
        }
    }

    private void UpdateMoveSpeed()
    {
        if (_currentHurtTime.Equals(hurtTime))
            return;
        
        _currentHurtTime += Mathf.Min(hurtTime, _currentHurtTime + Time.fixedDeltaTime);
        _currentMoveSpeed = Mathf.Lerp(minMoveSpeedWhenHurted, moveSpeed, _currentHurtTime / hurtTime);
    }

    public void OnBrickActivated()
    {
        _isMoving = true;
        _indexCoordinates = 0;
        _currentHurtTime = hurtTime;
        _currentMoveSpeed = moveSpeed;
    }
    
    public void OnBrickHurt()
    {
        _currentHurtTime = .0f;
    }
    
    public void OnBrickDeactivated()
    {
        _isMoving = false;
        _indexCoordinates = -1;
    }
    
}
