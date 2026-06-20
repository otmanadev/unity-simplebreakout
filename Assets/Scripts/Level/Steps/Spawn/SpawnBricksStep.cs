using System.Collections.Generic;
using UnityEngine;

public class SpawnBricksStep : LevelStep
{
    
    private List<Brick> _spawnedBricks;
    
    protected override void StartStep()
    {
        Debug.Log("[SpawnBricksStep] Spawning Bricks");
        BricksManager.Instance.SetOrUpdateLevelStep(this);
        SpawnAllBricks();
    }

    private void SpawnAllBricks()
    {
        _spawnedBricks = new List<Brick>(BricksManager.Instance.AllBricks);
        foreach (Brick brick in _spawnedBricks)
        {
            brick.StartBrickSpawn();
        }
    }
    
    public override void OnReceivedNotificationFromUnityObject(Object unityObject)
    {
        if (unityObject is not Brick)
        {
            Debug.LogError("[SpawnBricksStep] Notification was received by an Unity object but not a Brick.");
            return;
        }
        
        Brick brick = (Brick) unityObject;
        if (!_spawnedBricks.Contains(brick))
        {
            Debug.LogWarning($"[SpawnBricksStep] Received notification from Brick {brick.name} but was already spawned");
            return;
        }
        
        _spawnedBricks.Remove(brick);
        CheckIfAllBricksAreSpawnedBeforeNotifyLevelStepsGroup();
    }

    private void CheckIfAllBricksAreSpawnedBeforeNotifyLevelStepsGroup()
    {
        if (_spawnedBricks.Count > 0)
            return;
        
        BricksManager.Instance.ClearLevelStep();
        OnLevelStepFinishedNotification();
    }
    
}
