using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class PlatformsManager : MonoBehaviour
{
    
    public static PlatformsManager Instance;
    
    private List<Platform> _platforms;
    
    [Header("Platforms Datas")]
    [SerializeField] private List<PlatformSizeSO> platformSizes;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning($"[PlatformsManager / {name}] Instance is not unique : this instance will not be created");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        Assert.IsNotNull(platformSizes);
        _platforms = new List<Platform>();
    }

    /// <summary>
    /// Receive notification from platform when he is created.
    /// </summary>
    /// <param name="platform"></param>
    public void OnPlatformCreatedNotification(Platform platform)
    {
        if (_platforms.Contains(platform))
        {
            return;
        }
        
        _platforms.Add(platform);
        Debug.Log($"[PlatformsManager / {name}] Received notification from {platform.name} : Platform created.");
    }
    
    /// <summary>
    /// Enable given power up for every platforms.
    /// </summary>
    /// <param name="powerUpType"></param>
    public void ActivatePlatformPowerUp(EPowerUp powerUpType)
    {
        foreach (Platform platform in _platforms)
        {
            switch (powerUpType)
            {
                case EPowerUp.PLATFORM_BIGGER:
                case EPowerUp.PLATFORM_SMALLER:
                    PlatformSizeSO currentPlatformSize = platform.PlatformSizeSo;
                    PlatformSizeSO newPlatformSize = PickUpNewPlatformSizeSoForCurrentPlatform(powerUpType, currentPlatformSize);
                    platform.SetUpNewPlatformSize(newPlatformSize);
                    break;
            }
        }
    }

    /// <summary>
    /// Select platform size SO from current platform size and depending on selected power up.
    /// </summary>
    /// <param name="powerUpType"></param>
    /// <param name="currentPlatformSize"></param>
    /// <returns></returns>
    private PlatformSizeSO PickUpNewPlatformSizeSoForCurrentPlatform(EPowerUp powerUpType, PlatformSizeSO currentPlatformSize)
    {
        if (!powerUpType.Equals(EPowerUp.PLATFORM_SMALLER) && !powerUpType.Equals(EPowerUp.PLATFORM_BIGGER))
        {
            Debug.LogError($"[PlatformsManager / {name}] Cannot find PlatformSizeSO from power up type {powerUpType}");
            return null;
        }
        
        EPlatformSize currentPlatformSizeType = currentPlatformSize.PlatformSizeType;
        EPlatformSize newPlatformSizeType = currentPlatformSizeType;
        
        switch (currentPlatformSizeType)
        {
            case EPlatformSize.XLARGE:
                newPlatformSizeType = powerUpType.Equals(EPowerUp.PLATFORM_SMALLER)
                    ? EPlatformSize.LARGE
                    : newPlatformSizeType;
                break;
            case EPlatformSize.LARGE:
                newPlatformSizeType = powerUpType.Equals(EPowerUp.PLATFORM_BIGGER)
                    ? EPlatformSize.XLARGE
                    : EPlatformSize.MEDIUM;
                break;
            case EPlatformSize.MEDIUM:
                newPlatformSizeType = powerUpType.Equals(EPowerUp.PLATFORM_BIGGER)
                    ? EPlatformSize.LARGE
                    : EPlatformSize.SMALL;
                break;
            case EPlatformSize.SMALL:
                newPlatformSizeType = powerUpType.Equals(EPowerUp.PLATFORM_BIGGER)
                    ? EPlatformSize.MEDIUM
                    : EPlatformSize.XSMALL;
                break;
            case EPlatformSize.XSMALL:
                newPlatformSizeType = powerUpType.Equals(EPowerUp.PLATFORM_BIGGER)
                    ? EPlatformSize.SMALL
                    : newPlatformSizeType;
                break;
        }

        PlatformSizeSO newPlatformSizeSo = platformSizes.Find(e => e.PlatformSizeType.Equals(newPlatformSizeType));
        if (newPlatformSizeSo == null)
        {
            Debug.LogError($"[PlatformsManager / {name}] Cannot find PlatformSizeSO from size type {newPlatformSizeType}");
            return null;
        }

        return newPlatformSizeSo;
    }
    
}
