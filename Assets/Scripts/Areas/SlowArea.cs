using UnityEngine;

public class SlowArea : Area, IBallTriggerHandler, IBulletTriggerHandler
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

    public TriggerResponse HandleBallTriggerEnter(Collider2D _, Ball ball)
    {
        ball.Movement.AddMovementBonus(movementBonus);
        return new TriggerResponse();
    }

    public TriggerResponse HandleBallTriggerExit(Collider2D _, Ball ball)
    {
        ball.Movement.RemoveMovementBonus(movementBonus);
        return new TriggerResponse();
    }

    public TriggerResponse HandleBulletTriggerEnter(Collider2D _, Bullet bullet)
    {
        bullet.Movement.AddMovementBonus(movementBonus);
        return new TriggerResponse();
    }

    public TriggerResponse HandleBulletTriggerExit(Collider2D _, Bullet bullet)
    {
        bullet.Movement.RemoveMovementBonus(movementBonus);
        return new TriggerResponse();
    }
    
}
