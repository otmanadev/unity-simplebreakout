using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class InGameControls : MonoBehaviour
{

    [SerializeField] private Platform platform;

    private void Awake()
    {
        Assert.IsNotNull(platform);
    }

    public void OnPlatformMove(InputAction.CallbackContext context)
    {
        if (context.started) return;
        
        var directionValue = context.ReadValue<Vector2>().x;
        
        platform.InputHorizontalDirection = directionValue;
    }
    
}
