using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBallsStep : LevelStep
{
    
    private List<Ball> _spawnedBalls;
    
    protected override void StartStep()
    {
        Debug.Log("[SpawnBallsStep] Spawning Balls");
        BallsManager.Instance.SetOrUpdateLevelStep(this);
        SpawnAllBalls();
    }

    private void SpawnAllBalls()
    {
        _spawnedBalls = new List<Ball>(BallsManager.Instance.AllBalls);
        foreach (Ball ball in _spawnedBalls)
        {
            ball.SpawnBall();
        }
    }
    
    public override void OnReceivedNotificationFromUnityObject(Object unityObject)
    {
        if (unityObject is not Ball)
        {
            Debug.LogError("[SpawnBallsStep] Notification was received by an Unity object but not a Ball.");
            return;
        }
        
        Ball ball = (Ball) unityObject;
        if (!_spawnedBalls.Contains(ball))
        {
            Debug.LogWarning($"[SpawnBallsStep] Received notification from Ball {ball.name} but was already spawned");
            return;
        }
        
        _spawnedBalls.Remove(ball);
        CheckIfAllBallsAreSpawnedBeforeNotifyLevelStepsGroup();
    }

    private void CheckIfAllBallsAreSpawnedBeforeNotifyLevelStepsGroup()
    {
        if (_spawnedBalls.Count > 0)
            return;
        
        BallsManager.Instance.ClearLevelStep();
        OnLevelStepFinishedNotification();
    }
    
}
