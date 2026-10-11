using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBallsStep : BallsStep
{
    
    protected override void LogStartBallsStep()
    {
        Debug.Log("[STEP / Spawn Balls] Apparition des balles en cours...");
    }

    protected override void LogEndBallsStep()
    {
        Debug.Log("[STEP / Spawn Balls] Apparition des balles terminé.");
    }

    protected override void AttachEventToBall(Ball ball)
    {
        ball.OnSpawnAnimationCompleted += OnReceiveNotificationFromBall;
        ball.SpawnBall();
    }

    protected override void DetachEventToBall(Ball ball)
    {
        ball.OnSpawnAnimationCompleted -= OnReceiveNotificationFromBall;
    }
    
}
