using UnityEngine;

public class DarkArea : Area
{
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject collidedObject = other.gameObject;

        if (IsTriggeredWithBallOrBullet(collidedObject))
            TryUpdateAlphaOnSpriteRenderer(collidedObject, .0f);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        GameObject collidedObject = other.gameObject;

        if (IsTriggeredWithBallOrBullet(collidedObject))
            TryUpdateAlphaOnSpriteRenderer(collidedObject, 1.0f);
    }

    private bool IsTriggeredWithBallOrBullet(GameObject collidedObject)
    {
        return collidedObject.TryGetComponent(out Ball _)
               || collidedObject.TryGetComponent(out Bullet _);
    }

    private void TryUpdateAlphaOnSpriteRenderer(GameObject collidedObject, float alpha)
    {
        if (!collidedObject.TryGetComponent<SpriteRenderer>(out SpriteRenderer spriteRenderer))
            return;
        
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;
    }
    
}
