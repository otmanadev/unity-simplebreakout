using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepawnBallsStep : BallsStep
{
    
    protected override void LogStartBallsStep()
    {
        Debug.Log("[STEP / Despawn Balls] Destruction des balles en cours...");
    }

    protected override void LogEndBallsStep()
    {
        Debug.Log("[STEP / Despawn Balls] Destruction des balles terminé.");
    }

    protected override void AttachEventToBall(Ball ball)
    {
        ball.OnDespawnAnimationCompleted += OnReceiveNotificationFromBall;
        ball.DespawnBall();
    }

    protected override void DetachEventToBall(Ball ball)
    {
        ball.OnDespawnAnimationCompleted -= OnReceiveNotificationFromBall;
    }
    
}
