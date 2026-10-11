using System.Collections.Generic;
using UnityEngine;

public class SpawnPlatformsStep : PlatformsStep
{
    
    protected override void LogStartPlatformsStep()
    {
        Debug.Log("[STEP / Spawn Platforms] Apparition des plateformes en cours...");
    }

    protected override void LogEndPlatformsStep()
    {
        Debug.Log("[STEP / Spawn Platforms] Apparition des plateformes terminé.");
    }

    protected override void AttachEventToPlatform(Platform platform)
    {
        platform.OnSpawnAnimationCompleted += OnReceiveNotificationFromPlatform;
        platform.SpawnPlatform();
    }

    protected override void DetachEventToPlatform(Platform platform)
    {
        platform.OnSpawnAnimationCompleted -= OnReceiveNotificationFromPlatform;
    }
    
}
