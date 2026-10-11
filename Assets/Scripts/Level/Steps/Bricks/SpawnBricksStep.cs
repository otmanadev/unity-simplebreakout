using System.Collections.Generic;
using UnityEngine;

public class SpawnBricksStep : BricksStep
{
    
    protected override void LogStartBricksStep()
    {
        Debug.Log("[STEP / Spawn Bricks] Apparition des bricks en cours...");
    }

    protected override void LogEndBricksStep()
    {
        Debug.Log("[STEP / Spawn Bricks] Apparition des bricks terminé.");
    }

    protected override void AttachEventToBrick(Brick brick)
    {
        brick.OnSpawnAnimationCompleted += OnReceiveNotificationFromBrick;
        brick.StartBrickSpawn();
    }

    protected override void DetachEventToBrick(Brick brick)
    {
        brick.OnSpawnAnimationCompleted -= OnReceiveNotificationFromBrick;
    }
    
}
