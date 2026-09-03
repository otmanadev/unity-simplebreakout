using UnityEngine;

public struct CollisionResponse
{
    
    public Vector2 ReflectedDirection;
    
    public CollisionResponse(Vector2 reflectedDirection)
    {
        ReflectedDirection = reflectedDirection;
    }
    
}