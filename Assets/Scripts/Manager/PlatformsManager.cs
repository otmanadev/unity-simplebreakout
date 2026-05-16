using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class PlatformsManager : MonoBehaviour
{
    
    public static PlatformsManager Instance;
    
    private List<Platform> _platforms;

    [Header("Controls")] 
    [SerializeField, UnityEngine.Range(.5f, 2f)] private float sensitivityHorizontalPlatformVelocity = 1.5f;
    
    [Header("Platforms Datas")]
    [SerializeField] private List<SOPlatformSize> platformSizes;

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

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        UpdateMousePositions();
    }

    private void UpdateMousePositions()
    {
        float delta = sensitivityHorizontalPlatformVelocity * Input.GetAxis("Mouse X");
        foreach (Platform platform in _platforms)
        {
            platform.InputHorizontalDirection = delta;
        }
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
                case EPowerUp.PlatformBigger:
                case EPowerUp.PlatformSmaller:
                    SOPlatformSize currentSoPlatformSize = platform.SoPlatformSize;
                    SOPlatformSize newSoPlatformSize = PickUpNewPlatformSizeSoForCurrentPlatform(powerUpType, currentSoPlatformSize);
                    platform.SetUpNewPlatformSize(newSoPlatformSize);
                    break;
            }
        }
    }

    /// <summary>
    /// Select platform size SO from current platform size and depending on selected power up.
    /// </summary>
    /// <param name="powerUpType"></param>
    /// <param name="currentSoPlatformSize"></param>
    /// <returns></returns>
    private SOPlatformSize PickUpNewPlatformSizeSoForCurrentPlatform(EPowerUp powerUpType, SOPlatformSize currentSoPlatformSize)
    {
        if (!powerUpType.Equals(EPowerUp.PlatformSmaller) && !powerUpType.Equals(EPowerUp.PlatformBigger))
        {
            Debug.LogError($"[PlatformsManager / {name}] Cannot find PlatformSizeSO from power up type {powerUpType}");
            return null;
        }
        
        EPlatformSize currentPlatformSizeType = currentSoPlatformSize.PlatformSize;
        EPlatformSize newPlatformSizeType = currentPlatformSizeType;
        
        switch (currentPlatformSizeType)
        {
            case EPlatformSize.ExtraLarge:
                newPlatformSizeType = powerUpType.Equals(EPowerUp.PlatformSmaller)
                    ? EPlatformSize.Large
                    : newPlatformSizeType;
                break;
            case EPlatformSize.Large:
                newPlatformSizeType = powerUpType.Equals(EPowerUp.PlatformBigger)
                    ? EPlatformSize.ExtraLarge
                    : EPlatformSize.Medium;
                break;
            case EPlatformSize.Medium:
                newPlatformSizeType = powerUpType.Equals(EPowerUp.PlatformBigger)
                    ? EPlatformSize.Large
                    : EPlatformSize.Small;
                break;
            case EPlatformSize.Small:
                newPlatformSizeType = powerUpType.Equals(EPowerUp.PlatformBigger)
                    ? EPlatformSize.Medium
                    : EPlatformSize.ExtraSmall;
                break;
            case EPlatformSize.ExtraSmall:
                newPlatformSizeType = powerUpType.Equals(EPowerUp.PlatformBigger)
                    ? EPlatformSize.Small
                    : newPlatformSizeType;
                break;
        }

        SOPlatformSize newSoPlatformSize = platformSizes.Find(e => e.PlatformSize.Equals(newPlatformSizeType));
        if (newSoPlatformSize == null)
        {
            Debug.LogError($"[PlatformsManager / {name}] Cannot find PlatformSizeSO from size type {newPlatformSizeType}");
            return null;
        }

        return newSoPlatformSize;
    }
    
}
