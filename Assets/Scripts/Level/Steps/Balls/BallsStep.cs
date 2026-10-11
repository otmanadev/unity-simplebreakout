
using System.Collections.Generic;
using UnityEngine;

public abstract class BallsStep : LevelStep
{
    
    private HashSet<Ball> _pendingBalls;
    
    protected override void StartStep()
    {
        LogStartBallsStep();
        
        List<Ball> balls = new(BallsManager.Instance.AllBalls);
        if (balls.Count == 0)
        {
            OnLevelStepFinishedNotification();
            return;
        }
        
        _pendingBalls = new HashSet<Ball>(balls);
        foreach (Ball ball in balls)
        {
            AttachEventToBall(ball);
        }
    }

    /// <summary>
    /// Réception d'une notification de la balle.
    /// </summary>
    /// <param name="ball"></param>
    public void OnReceiveNotificationFromBall(Ball ball)
    {
        if (!_pendingBalls.Remove(ball))
            return;
        
        DetachEventToBall(ball);
        
        if (_pendingBalls.Count > 0)
            return;
        
        LogEndBallsStep();
        OnLevelStepFinishedNotification();
    }

    /// <summary>
    /// Log le début de BallsStep.
    /// </summary>
    protected abstract void LogStartBallsStep();
    
    /// <summary>
    /// Log la fin de BallsStep.
    /// </summary>
    protected abstract void LogEndBallsStep();
    
    /// <summary>
    /// Crée un event entre BallsStep et la balle.
    /// </summary>
    /// <param name="ball"></param>
    protected abstract void AttachEventToBall(Ball ball);
    
    /// <summary>
    /// Détache l'event entre BallsStep et la balle.
    /// </summary>
    /// <param name="ball"></param>
    protected abstract void DetachEventToBall(Ball ball);
    
}