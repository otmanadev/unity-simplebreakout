using System.Collections.Generic;
using UnityEngine;

public class SpawnPlatformsStep : LevelStep
{
    
    private List<Platform> _spawnedPlatforms;
    
    protected override void StartStep()
    {
        Debug.Log("[SpawnPlatformsStep] Spawning Platforms");
        PlatformsManager.Instance.SetOrUpdateLevelStep(this);
        SpawnAllPlatforms();
    }

    private void SpawnAllPlatforms()
    {
        _spawnedPlatforms = new List<Platform>(PlatformsManager.Instance.AllPlatforms);
        foreach (Platform platform in _spawnedPlatforms)
        {
            platform.SpawnPlatform();
        }
    }
    
    public override void OnReceivedNotificationFromUnityObject(Object unityObject)
    {
        if (unityObject is not Platform)
        {
            Debug.LogError("[SpawnPlatformsStep] Notification was received by an Unity object but not a Platform.");
            return;
        }
        
        Platform platform = (Platform) unityObject;
        if (!_spawnedPlatforms.Contains(platform))
        {
            Debug.LogWarning($"[SpawnPlatformsStep] Received notification from Platform {platform.name} but was already spawned");
            return;
        }
        
        _spawnedPlatforms.Remove(platform);
        CheckIfAllPlatformsAreSpawnedBeforeNotifyLevelStepsGroup();
    }

    private void CheckIfAllPlatformsAreSpawnedBeforeNotifyLevelStepsGroup()
    {
        if (_spawnedPlatforms.Count > 0)
            return;
        
        PlatformsManager.Instance.ClearLevelStep();
        OnLevelStepFinishedNotification();
    }
    
}
