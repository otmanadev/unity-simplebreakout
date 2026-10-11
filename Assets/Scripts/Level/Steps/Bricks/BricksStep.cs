
using System.Collections.Generic;
using UnityEngine;

public abstract class BricksStep : LevelStep
{
    
    private HashSet<Brick> _pendingBricks;
    
    protected override void StartStep()
    {
        LogStartBricksStep();
        
        List<Brick> bricks = new(BricksManager.Instance.AllBricks);
        if (bricks.Count == 0)
        {
            OnLevelStepFinishedNotification();
            return;
        }
        
        _pendingBricks = new HashSet<Brick>(bricks);
        foreach (Brick brick in bricks)
        {
            AttachEventToBrick(brick);
        }
    }

    /// <summary>
    /// Réception d'une notification de la balle.
    /// </summary>
    /// <param name="brick"></param>
    public void OnReceiveNotificationFromBrick(Brick brick)
    {
        if (!_pendingBricks.Remove(brick))
            return;
        
        DetachEventToBrick(brick);
        
        if (_pendingBricks.Count > 0)
            return;
        
        LogEndBricksStep();
        OnLevelStepFinishedNotification();
    }

    /// <summary>
    /// Log le début de BricksStep.
    /// </summary>
    protected abstract void LogStartBricksStep();
    
    /// <summary>
    /// Log la fin de BricksStep.
    /// </summary>
    protected abstract void LogEndBricksStep();
    
    /// <summary>
    /// Crée un event entre BricksStep et la brick.
    /// </summary>
    /// <param name="brick"></param>
    protected abstract void AttachEventToBrick(Brick brick);
    
    /// <summary>
    /// Détache l'event entre BricksStep et la brick.
    /// </summary>
    /// <param name="brick"></param>
    protected abstract void DetachEventToBrick(Brick brick);
}