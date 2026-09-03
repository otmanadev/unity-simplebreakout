using UnityEngine;

public class DarkArea : Area, IBallTriggerHandler, IBulletTriggerHandler
{
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject collidedObject = other.gameObject;
        
        if (collidedObject.TryGetComponent(out Ball ball))
            ball.RegisterTriggerEnter(this, other);
        
        if (collidedObject.TryGetComponent(out Bullet bullet))
            bullet.RegisterTriggerEnter(this, other);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        GameObject collidedObject = other.gameObject;
        
        if (collidedObject.TryGetComponent(out Ball ball))
            ball.RegisterTriggerExit(this, other);
        
        if (collidedObject.TryGetComponent(out Bullet bullet))
            bullet.RegisterTriggerExit(this, other);
    }

    private void TryUpdateAlphaOnSpriteRenderer(GameObject collidedObject, float alpha)
    {
        if (!collidedObject.TryGetComponent(out SpriteRenderer spriteRenderer))
            return;
        
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }

    public TriggerResponse HandleBallTriggerEnter(Collider2D _, Ball ball)
    {
        TryUpdateAlphaOnSpriteRenderer(ball.gameObject, .0f);
        return new TriggerResponse();
    }

    public TriggerResponse HandleBallTriggerExit(Collider2D _, Ball ball)
    {
        TryUpdateAlphaOnSpriteRenderer(ball.gameObject, 1.0f);
        return new TriggerResponse();
    }

    public TriggerResponse HandleBulletTriggerEnter(Collider2D _, Bullet bullet)
    {
        TryUpdateAlphaOnSpriteRenderer(bullet.gameObject, .0f);
        return new TriggerResponse();
    }

    public TriggerResponse HandleBulletTriggerExit(Collider2D _, Bullet bullet)
    {
        TryUpdateAlphaOnSpriteRenderer(bullet.gameObject, 1.0f);
        return new TriggerResponse();
    }
    
}
