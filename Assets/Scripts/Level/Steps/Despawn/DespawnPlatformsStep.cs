using System.Collections.Generic;
using UnityEngine;

public class DespawnPlatformsStep : LevelStep
{
    
    private List<Platform> _despawnedPlatforms;
    
    protected override void StartStep()
    {
        Debug.Log("[DespawnPlatformsStep] Despawning Platforms");
        PlatformsManager.Instance.SetOrUpdateLevelStep(this);
        DespawnAllPlatforms();
    }

    private void DespawnAllPlatforms()
    {
        _despawnedPlatforms = new List<Platform>(PlatformsManager.Instance.AllPlatforms);
        if (_despawnedPlatforms.Count == 0)
        {
            Debug.LogWarning($"[DespawnPlatformsStep] There is no platforms to despawn");
            NotifyLevelStepsGroup();
            return;
        }
        
        foreach (Platform platform in _despawnedPlatforms)
        {
            // TODO
            //platform.DespawnPlatform();
        }
    }
    
    public override void OnReceivedNotificationFromUnityObject(Object unityObject)
    {
        if (unityObject is not Platform)
        {
            Debug.LogError("[DespawnPlatformsStep] Notification was received by an Unity object but not a Platform.");
            return;
        }
        
        Platform platform = (Platform) unityObject;
        if (!_despawnedPlatforms.Contains(platform))
        {
            Debug.LogWarning($"[DespawnPlatformsStep] Received notification from Platform {platform.name} but was already despawned");
            return;
        }
        
        _despawnedPlatforms.Remove(platform);
        CheckIfAllPlatformsAreDespawnedBeforeNotifyLevelStepsGroup();
    }

    private void CheckIfAllPlatformsAreDespawnedBeforeNotifyLevelStepsGroup()
    {
        if (_despawnedPlatforms.Count > 0)
            return;

        NotifyLevelStepsGroup();
    }

    private void NotifyLevelStepsGroup()
    {
        PlatformsManager.Instance.ClearLevelStep();
        OnLevelStepFinishedNotification();
    }
    
}
