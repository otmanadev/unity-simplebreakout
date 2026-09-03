using System;
using NUnit.Framework;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(CompositeCollider2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(SpriteRenderer))]
public class KeyArea : MonoBehaviour, IBallTriggerHandler, IBulletTriggerHandler
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
        
        _spriteRenderer.color = affectedArea.AreaColor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject collidedObject = other.gameObject;
        
        if (collidedObject.TryGetComponent(out Ball ball))
            ball.RegisterTriggerEnter(this, other);
        
        if (collidedObject.TryGetComponent(out Bullet bullet))
            bullet.RegisterTriggerEnter(this, other);
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
        }
    }

    /// <summary>
    /// Récupère la couleur de l'Area en fonction de son activité.
    /// </summary>
    private void UpdateColor()
    {
        _spriteRenderer.color = affectedArea.AreaColor;
    }

    private void SetAreaActive()
    {
        if (!isActive)
            isActive = true;
        _animator.SetTrigger(AnimationTriggerActive);
        affectedArea.EnableArea();
        UpdateColor();
    }

    private void SetAreaInactive()
    {
        if (isActive)
            isActive = false;
        _animator.SetTrigger(AnimationTriggerInactive);
        affectedArea.DisableArea();
        UpdateColor();
    }

    private void OnValidate()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (affectedArea != null)
            UpdateColor();
    }

    public TriggerResponse HandleBallTriggerEnter(Collider2D _, Ball __)
    {
        SwitchAreaActiveProperties();
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

    public TriggerResponse HandleBulletTriggerEnter(Collider2D _, Bullet __)
    {
        SwitchAreaActiveProperties();
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
