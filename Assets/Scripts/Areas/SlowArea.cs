using UnityEngine;

public class SlowArea : Area
{
    
    [Header("Slow Area Properties")]
    [SerializeField] private MovementBonus movementBonus;

    protected override void Awake()
    {
        base.Awake();
        movementBonus.InitializeInstanceId(GetInstanceID());
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject collidedObject = other.gameObject;

        if (IsTriggeredWithBallOrBullet(collidedObject))
            AddMovementBonusOnCollidedObject(collidedObject);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        GameObject collidedObject = other.gameObject;

        if (IsTriggeredWithBallOrBullet(collidedObject))
            RemoveMovementBonusOnCollidedObject(collidedObject);
    }

    private bool IsTriggeredWithBallOrBullet(GameObject collidedObject)
    {
        return collidedObject.TryGetComponent(out Ball _)
               || collidedObject.TryGetComponent(out Bullet _);
    }

    private void AddMovementBonusOnCollidedObject(GameObject collidedObject)
    {
        if (collidedObject.TryGetComponent(out Ball ball))
        {
            ball.Movement.AddMovementBonus(movementBonus);
            return;
        }
        
        if (collidedObject.TryGetComponent(out Bullet bullet))
        {
            bullet.Movement.AddMovementBonus(movementBonus);
        }
    }
    
    private void RemoveMovementBonusOnCollidedObject(GameObject collidedObject)
    {
        if (collidedObject.TryGetComponent(out Ball ball))
        {
            ball.Movement.RemoveMovementBonus(movementBonus);
            return;
        }
        
        if (collidedObject.TryGetComponent(out Bullet bullet))
        {
            bullet.Movement.RemoveMovementBonus(movementBonus);
        }
    }
    
}
