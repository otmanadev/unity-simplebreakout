using System.Collections.Generic;
using UnityEngine;

public class DespawnPlatformsStep : PlatformsStep
{
    
    protected override void LogStartPlatformsStep()
    {
        Debug.Log("[STEP / Despawn Platforms] Destruction des plateformes en cours...");
    }

    protected override void LogEndPlatformsStep()
    {
        Debug.Log("[STEP / Despawn Platforms] Destruction des plateformes terminé.");
    }

    protected override void AttachEventToPlatform(Platform platform)
    {
        platform.OnDespawnAnimationCompleted += OnReceiveNotificationFromPlatform;
        // TODO : Animation destruction plateforme à faire
        DetachEventToPlatform(platform);
    }

    protected override void DetachEventToPlatform(Platform platform)
    {
        platform.OnDespawnAnimationCompleted -= OnReceiveNotificationFromPlatform;
        // TODO : Animation destruction plateforme à faire
        Destroy(platform);
    }
    
}
