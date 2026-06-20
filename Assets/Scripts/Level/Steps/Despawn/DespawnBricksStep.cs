using System.Collections.Generic;
using UnityEngine;

public class DespawnBricksStep : LevelStep
{
    
    private List<Brick> _despawnedBricks;
    
    protected override void StartStep()
    {
        Debug.Log("[DespawnBricksStep] Despawning Bricks");
        BricksManager.Instance.SetOrUpdateLevelStep(this);
        DespawnAllBricks();
    }

    private void DespawnAllBricks()
    {
        _despawnedBricks = new List<Brick>(BricksManager.Instance.AllBricks);
        if (_despawnedBricks.Count == 0)
        {
            Debug.LogWarning($"[DespawnBricksStep] There is no bricks to despawn");
            NotifyLevelStepsGroup();
            return;
        }
        
        foreach (Brick brick in _despawnedBricks)
        {
            // TODO
            //brick.DespawnBrick();
        }
    }
    
    public override void OnReceivedNotificationFromUnityObject(Object unityObject)
    {
        if (unityObject is not Brick)
        {
            Debug.LogError("[DespawnBricksStep] Notification was received by an Unity object but not a Brick.");
            return;
        }
        
        Brick brick = (Brick) unityObject;
        if (!_despawnedBricks.Contains(brick))
        {
            Debug.LogWarning($"[DespawnBricksStep] Received notification from Brick {brick.name} but was already despawned");
            return;
        }
        
        _despawnedBricks.Remove(brick);
        CheckIfAllBricksAreDespawnedBeforeNotifyLevelStepsGroup();
    }

    private void CheckIfAllBricksAreDespawnedBeforeNotifyLevelStepsGroup()
    {
        if (_despawnedBricks.Count > 0)
            return;

        NotifyLevelStepsGroup();
    }
    
    private void NotifyLevelStepsGroup()
    {
        PlatformsManager.Instance.ClearLevelStep();
        OnLevelStepFinishedNotification();
    }
    
}
