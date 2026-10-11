
using System.Collections.Generic;

public abstract class PlatformsStep : LevelStep
{
    
    private HashSet<Platform> _pendingPlatforms;
    
    protected override void StartStep()
    {
        LogStartPlatformsStep();
        
        List<Platform> platforms = new(PlatformsManager.Instance.AllPlatforms);
        if (platforms.Count == 0)
        {
            OnLevelStepFinishedNotification();
            return;
        }
        
        _pendingPlatforms = new HashSet<Platform>(platforms);
        foreach (Platform platform in platforms)
        {
            AttachEventToPlatform(platform);
        }
    }

    /// <summary>
    /// Réception d'une notification de la balle.
    /// </summary>
    /// <param name="platform"></param>
    public void OnReceiveNotificationFromPlatform(Platform platform)
    {
        if (!_pendingPlatforms.Remove(platform))
            return;
        
        DetachEventToPlatform(platform);
        
        if (_pendingPlatforms.Count > 0)
            return;
        
        LogEndPlatformsStep();
        OnLevelStepFinishedNotification();
    }

    /// <summary>
    /// Log le début de PlatformsStep.
    /// </summary>
    protected abstract void LogStartPlatformsStep();
    
    /// <summary>
    /// Log la fin de PlatformsStep.
    /// </summary>
    protected abstract void LogEndPlatformsStep();
    
    /// <summary>
    /// Crée un event entre PlatformsStep et la plateforme.
    /// </summary>
    /// <param name="platform"></param>
    protected abstract void AttachEventToPlatform(Platform platform);
    
    /// <summary>
    /// Détache l'event entre PlatformsStep et la plateforme.
    /// </summary>
    /// <param name="platform"></param>
    protected abstract void DetachEventToPlatform(Platform platform);
    
}