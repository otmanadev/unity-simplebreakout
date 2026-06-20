using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepawnBallsStep : LevelStep
{
    
    private List<Ball> _despawnedBalls;
    
    protected override void StartStep()
    {
        Debug.Log("[DepawnBallsStep] Despawning Balls");
        BallsManager.Instance.SetOrUpdateLevelStep(this);
        DespawnAllBalls();
    }

    private void DespawnAllBalls()
    {
        _despawnedBalls = new List<Ball>(BallsManager.Instance.AllBalls);
        if (_despawnedBalls.Count == 0)
        {
            Debug.LogWarning($"[DepawnBallsStep] There is no balls to despawn");
            NotifyLevelStepsGroup();
            return;
        }
        
        foreach (Ball ball in _despawnedBalls)
        {
            ball.DespawnBall();
        }
    }
    
    public override void OnReceivedNotificationFromUnityObject(Object unityObject)
    {
        if (unityObject is not Ball)
        {
            Debug.LogError("[DepawnBallsStep] Notification was received by an Unity object but not a Ball.");
            return;
        }
        
        Ball ball = (Ball) unityObject;
        if (!_despawnedBalls.Contains(ball))
        {
            Debug.LogWarning($"[DepawnBallsStep] Received notification from Ball {ball.name} but was already despawned");
            return;
        }
        
        _despawnedBalls.Remove(ball);
        CheckIfAllBallsAreDespawnedBeforeNotifyLevelStepsGroup();
    }

    private void CheckIfAllBallsAreDespawnedBeforeNotifyLevelStepsGroup()
    {
        if (_despawnedBalls.Count > 0)
            return;

        NotifyLevelStepsGroup();
    }
    
    private void NotifyLevelStepsGroup()
    {
        PlatformsManager.Instance.ClearLevelStep();
        OnLevelStepFinishedNotification();
    }
    
}
