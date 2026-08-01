using System;
using NUnit.Framework;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(CompositeCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class KeyArea : MonoBehaviour
{
    
    private static readonly String AnimationTriggerActive = "ActiveTrigger";
    private static readonly String AnimationTriggerInactive = "InactiveTrigger";
    
    [Header("References")]
    [SerializeField] private Area affectedArea;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    [Header("Properties")] 
    [SerializeField] private bool isActive;

    private void Awake()
    {
        Assert.IsNotNull(affectedArea);
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        if (isActive)
            SetAreaActive();
        
        if (!isActive)
            SetAreaInactive();
        
        _spriteRenderer.color = affectedArea.color;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject collidedObject = other.gameObject;

        if (IsTriggeredWithBallOrBullet(collidedObject))
            SwitchAreaActiveProperties();
    }
    
    private bool IsTriggeredWithBallOrBullet(GameObject collidedObject)
    {
        return collidedObject.TryGetComponent(out Ball _)
               || collidedObject.TryGetComponent(out Bullet _);
    }

    private void SwitchAreaActiveProperties()
    {
        if (isActive)
        {
            SetAreaInactive();
            return;
        }

        if (!isActive)
        {
            SetAreaActive();
            return;
        }
    }

    private void SetAreaActive()
    {
        if (!isActive)
            isActive = true;
        _animator.SetTrigger(AnimationTriggerActive);
        affectedArea.EnableArea();
    }

    private void SetAreaInactive()
    {
        if (isActive)
            isActive = false;
        _animator.SetTrigger(AnimationTriggerInactive);
        affectedArea.DisableArea();
    }
    
}
