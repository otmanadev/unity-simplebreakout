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
        EPlatformSize currentPlatformSizeType = currentPlatformSize.PlatformSizeType;
        EPlatformSize newPlatformSizeType = currentPlatformSizeType;
        switch (currentPlatformSizeType)
        {
            case EPlatformSize.XLARGE:
                if (powerUpType.Equals(EPowerUp.PLATFORM_SMALLER))
                    newPlatformSizeType = EPlatformSize.LARGE;
                break;
            case EPlatformSize.LARGE:
                if (powerUpType.Equals(EPowerUp.PLATFORM_BIGGER))
                    newPlatformSizeType = EPlatformSize.XLARGE;
                if (powerUpType.Equals(EPowerUp.PLATFORM_SMALLER))
                    newPlatformSizeType = EPlatformSize.MEDIUM;
                break;
            case EPlatformSize.MEDIUM:
                if (powerUpType.Equals(EPowerUp.PLATFORM_BIGGER))
                    newPlatformSizeType = EPlatformSize.LARGE;
                if (powerUpType.Equals(EPowerUp.PLATFORM_SMALLER))
                    newPlatformSizeType = EPlatformSize.SMALL;
                break;
            case EPlatformSize.SMALL:
                if (powerUpType.Equals(EPowerUp.PLATFORM_BIGGER))
                    newPlatformSizeType = EPlatformSize.MEDIUM;
                if (powerUpType.Equals(EPowerUp.PLATFORM_SMALLER))
                    newPlatformSizeType = EPlatformSize.XSMALL;
                break;
            case EPlatformSize.XSMALL:
                if (powerUpType.Equals(EPowerUp.PLATFORM_BIGGER))
                    newPlatformSizeType = EPlatformSize.SMALL;
                break;
        }

        PlatformSizeSO newPlatformSizeSO = platformSizes.Find(e => e.PlatformSizeType.Equals(newPlatformSizeType));
        if (newPlatformSizeSO == null)
        {
            Debug.LogError($"[PlatformsManager / {name}] Cannot find PlatformSizeSO from its type {newPlatformSizeType}");
            return null;
        }

        return newPlatformSizeSO;
    }
    
}
