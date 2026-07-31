using UnityEngine;

public class SlowArea : Area
{
    
    [Header("Slow Area Properties")]
    [SerializeField, Range(0.0f, 100.0f)] private float slowPercentage = 75f;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject collidedObject = other.gameObject;

        if (IsTriggeredWithBallOrBullet(collidedObject))
            TryUpdateMovementSpeedPercentageOfGameObject(collidedObject, slowPercentage);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        GameObject collidedObject = other.gameObject;

        if (IsTriggeredWithBallOrBullet(collidedObject))
            TryUpdateMovementSpeedPercentageOfGameObject(collidedObject, .0f);
    }

    private bool IsTriggeredWithBallOrBullet(GameObject collidedObject)
    {
        return collidedObject.TryGetComponent(out Ball _)
               || collidedObject.TryGetComponent(out Bullet _);
    }

    private void TryUpdateMovementSpeedPercentageOfGameObject(GameObject collidedObject, float movementSpeedPercentage)
    {
        if (collidedObject.TryGetComponent(out Ball ball))
        {
            ball.movementSpeedPercentage = GetMovementSpeedPercentage(movementSpeedPercentage);
            return;
        }
        
        if (collidedObject.TryGetComponent(out Bullet bullet))
        {
            bullet.movementSpeedPercentage = GetMovementSpeedPercentage(movementSpeedPercentage);
        }
    }

    private float GetMovementSpeedPercentage(float movementSpeedPercentage)
    {
        return (100.0f - movementSpeedPercentage) / 100.0f;
    }
    
}
