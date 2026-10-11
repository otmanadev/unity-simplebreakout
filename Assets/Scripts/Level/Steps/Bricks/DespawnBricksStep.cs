using System.Collections.Generic;
using UnityEngine;

public class DespawnBricksStep : BricksStep
{
    
    protected override void LogStartBricksStep()
    {
        Debug.Log("[STEP / Despawn Bricks] Destruction des bricks en cours...");
    }

    protected override void LogEndBricksStep()
    {
        Debug.Log("[STEP / Despawn Bricks] Destruction des bricks terminé.");
    }

    protected override void AttachEventToBrick(Brick brick)
    {
        brick.OnDespawnAnimationCompleted += OnReceiveNotificationFromBrick;
        // TODO : Animation destruction brique à faire
        DetachEventToBrick(brick);
    }

    protected override void DetachEventToBrick(Brick brick)
    {
        brick.OnDespawnAnimationCompleted -= OnReceiveNotificationFromBrick;
        // TODO : Animation destruction brique à faire
        Destroy(brick);
    }
    
}
